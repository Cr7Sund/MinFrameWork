using System;
using System.Reflection;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Package.Impl
{
    public class Injector : IInjector
    {

        private static readonly IInjectorFactory _factory = new InjectorFactory();
        private int _depth;
        private const int MAX_DEPTH = 100;

        public IPoolBinder PoolBinder { get;  }
        public IInjectorFactory Factory { get; set; }
        public IInjectionBinder Binder { get; set; }
        public IReflectionBinder Reflector { get; set; }


        public Injector()
        {
            IReflectionBinder reflectionBinder = new InjectorReflectionBinder();
            IPoolBinder poolBinder = new PoolBinder();

            Factory = _factory;
            Reflector = reflectionBinder;
            PoolBinder = poolBinder;
        }


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
                    failIf(pool == null, InjectionExceptionType.NOPOOL_CONSTRUCT);

                    pool.InflationType = PoolInflationType.INCREMENT;
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
                        retVal = InjectInternal(retVal);
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
            _depth = 0;
            return InjectInternal(target);
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
                else if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public void Deject(object target)
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

            PerformDejection(target, reflection);
        }

        private object InjectInternal(object target)
        {
            failIf(target == null, InjectionExceptionType.NULL_TARGET_INJECT, null, target);

            var t = target.GetType();

            failIf(Binder == null, InjectionExceptionType.NO_BINDER_INJECT, t, target);
            failIf(Reflector == null, InjectionExceptionType.NO_REFLECTOR, t, target);

            //Some things can't be injected into. Bail out.
            if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string))
            {
                return target;
            }
            _depth++;
            failIf(_depth > MAX_DEPTH, InjectionExceptionType.INJECT_DEPTH_LIMIT, t, target);


            var reflection = Reflector.Get(t);

            PerformFieldInjection(target, reflection);
            PostInject(target, reflection);
            return target;
        }

        private void PerformDejection(object target, IReflectedClass reflection)
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
                float depth = _depth;
                object value = GetValueInjection(pair.Item1, pair.Item2, target);
                InjectValueIntoPoint(value, target, pair.Item3);
                _depth = _depth > depth ? _depth - 1 : _depth;
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
                throw new MyException(message, type);
            }
        }

        private object GetValueInjection(Type t, object name, object target)
        {
            var binding = Binder.GetBinding(t, name);
            failIf(binding == null, InjectionExceptionType.NULL_BINDING_GET_INJECT, t, name, target);
            object bindingValue = binding.Value.SingleValue;

            object retVal;
            if (binding.Type == InjectionBindingType.VALUE)
            {
                if (!binding.IsToInject)
                {
                    retVal = bindingValue;
                }
                else
                {
                    binding.ToInject(false);
                    retVal = InjectInternal(bindingValue);
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
