using Cr7Sund.Framework.Api;
using System;
using System.Reflection;
namespace Cr7Sund.Framework.Impl
{
    public class Injector : IInjector
    {

        private static readonly IInjectorFactory _factory = new InjectorFactory();
        private readonly IPoolBinder _poolBinder;
        private readonly IReflectionBinder _reflectionBinder;

        public Injector()
        {
            _reflectionBinder = new InjectorReflectionBinder();
            _poolBinder = new PoolBinder();

            Factory = _factory;
            Reflector = _reflectionBinder;
            PoolBinder = _poolBinder;
        }
        public IPoolBinder PoolBinder { get; set; }
        public IInjectorFactory Factory { get; set; }
        public IInjectionBinder Binder { get; set; }
        public IReflectionBinder Reflector { get; set; }

        public object Instantiate(IInjectionBinding binding)
        {
            failIf(Binder == null, InjectionExceptionType.NO_BINDER_INSTANTIATE);
            failIf(Factory == null, InjectionExceptionType.NO_FACTORY_INSTANTIATE);

            object retVal = null;
            Type reflectionType = null;

            object bindingValue = binding.Value.SingleValue;
            if (bindingValue is not Type)
            {
                retVal = bindingValue;
            }
            else
            {
                reflectionType = bindingValue as Type;
            }

            if (retVal == null) // If we don't have an existing value , go ahead and create one
            {
                var reflection = Reflector.Get(reflectionType);
                // Actually we dont support construct inject
                failIf(reflection == null, InjectionExceptionType.NULL_REFLECTION_INSTANTIATE, reflectionType, null);
                failIf(reflection.Constructor == null, InjectionExceptionType.NULL_CONSTRUCTOR, reflectionType, null);
                failIf(reflection.ConstructorParameterCount > 0, InjectionExceptionType.NONEMPTY_CONSTRUCTOR, reflectionType, null);

                if (binding.Type == InjectionBindingType.POOL)
                {
                    var pool = PoolBinder.GetOrCreate(reflectionType);
                    pool.InflationType = PoolInflationType.INCREMENT;
                    failIf(pool == null, InjectionExceptionType.NOPOOL_CONSTRUCT);
                    retVal = pool.GetInstance();
                }
                else // handle other case
                {
                    retVal = Factory.Get(binding, null);
                }

                //If the InjectorFactory returns null, just return it. Otherwise inject the retVal if it needs it
                //This could happen if Activator.CreateInstance returns null
                if (retVal != null)
                {
                    if (binding.IsToInject)
                    {
                        retVal = Inject(retVal);
                    }

                    if (binding.Type == InjectionBindingType.SINGLETON || binding.Type == InjectionBindingType.VALUE)
                    {
                        binding.ToInject(false);
                    }
                }
            }

            failIf(retVal == null, InjectionExceptionType.NULL_INSTANTIATE_RESULT, binding.Key.SingleValue as Type, null);

            return retVal;
        }

        public object Inject(object target)
        {
            var t = target.GetType();

            failIf(Binder == null, InjectionExceptionType.NO_BINDER_INJECT, t, target);
            failIf(Reflector == null, InjectionExceptionType.NO_REFLECTOR, t, target);
            failIf(target == null, InjectionExceptionType.NULL_TARGET_INJECT, t, target);

            //Some things can't be injected into. Bail out.
            if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string))
            {
                return target;
            }

            var reflection = Reflector.Get(t);

            PerformFieldInjection(target, reflection);
            PostInject(target, reflection);
            return target;
        }

        public void Destroy(object instance)
        {
            if (instance != null)
            {
                var pool = PoolBinder.Get(instance.GetType());
                if (pool != null)
                {
                    pool.ReturnInstance(instance);
                }
            }
        }

        public void Uninject(object target)
        {
            failIf(Binder == null, InjectionExceptionType.NO_BINDER_UnINJECT);
            failIf(Reflector == null, InjectionExceptionType.NO_REFLECTOR_UNINJECT);
            failIf(target == null, InjectionExceptionType.NULL_TARGET_UNINJECT);

            var t = target.GetType();
            if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string))
            {
                return;
            }

            var reflection = Reflector.Get(t);

            PerformUninjection(target, reflection);
        }

        private void PerformUninjection(object target, IReflectedClass reflection)
        {
            for (int i = 0; i < reflection.Fields.Length; i++)
            {
                var pair = reflection.Fields[i];
                pair.Item3.SetValue(target, null);
            }
        }


        //After injection, call any methods labelled with the [PostConstruct] tag
        private void PostInject(object target, IReflectedClass reflection)
        {
            failIf(target == null, InjectionExceptionType.NULL_TARGET_POSTINJECT);
            failIf(reflection == null, InjectionExceptionType.NULL_REFLECTION_POSTINJECT);

            if (reflection.PostConstructor != null)
                reflection.PostConstructor.Invoke(target, null);
        }

        private void PerformFieldInjection(object target, IReflectedClass reflection)
        {
            failIf(target == null, InjectionExceptionType.NULL_TARGET_FIELDINJECT);
            failIf(reflection == null, InjectionExceptionType.NULL_REFLECTION_FIELDINJECT);

            for (int i = 0; i < reflection.Fields.Length; i++)
            {
                var pair = reflection.Fields[i];
                object value = GetValueInjection(pair.Item1, pair.Item2, target);
                InjectValueIntoPoint(value, target, pair.Item3);
            }
        }

        //Inject the value into the target at the specified injection point
        private void InjectValueIntoPoint(object value, object target, FieldInfo point)
        {
            failIf(point == null, InjectionExceptionType.NULL_INJECTION_POINT);
            failIf(value == null, InjectionExceptionType.NULL_VALUE_INJECTION);

            point.SetValue(target, value);
        }

        private void failIf(bool condition, InjectionExceptionType type)
        {
            failIf(condition, type, null, null, null);
        }

        private void failIf(bool condition, InjectionExceptionType type, Type t, object name)
        {
            failIf(condition, type, t, name, null);
        }

        private void failIf(bool condition, InjectionExceptionType type, Type t, object name, object target)
        {
            if (condition)
            {
                string message = $"ErrorCode: {type}";
                message += "\n\t\ttarget: " + target;
                message += "\n\t\ttype: " + t;
                message += "\n\t\tname: " + name;
                throw new Util.MyException(message, type);
            }
        }

        private object GetValueInjection(Type t, object name, object target)
        {
            var binding = Binder.GetBinding(t, name);
            failIf(binding == null, InjectionExceptionType.NULL_BINDING_GET_INJECT, t, name, target);
            object bindingValue = binding.Value.SingleValue;

            object retVal = null;
            if (binding.Type == InjectionBindingType.VALUE)
            {
                if (!binding.IsToInject)
                {
                    retVal = bindingValue;
                }
                else
                {
                    retVal = Inject(bindingValue);
                    binding.ToInject(false);
                }
            }
            else if (binding.Type == InjectionBindingType.SINGLETON)
            {
                if (bindingValue is Type || bindingValue == null)
                    Instantiate(binding);
                retVal = binding.Value.SingleValue;
            }
            else
            {
                // DEFAULT, always return a new value 
                retVal = Instantiate(binding);
            }

            return retVal;
        }
    }
}
