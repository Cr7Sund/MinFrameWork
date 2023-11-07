using System;
using System.Reflection;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class Injector : IInjector
    {
        public IInjectorFactory Factory { get; set; }
        public IInjectionBinder Binder { get; set; }
        public IReflectionBinder Reflector { get; set; }
        public IPoolBinder PoolBinder { get; set; }

        private static IInjectorFactory _factory = new InjectorFactory();
        private IReflectionBinder _reflectionBinder;
        private IPoolBinder _poolBinder;

        public Injector()
        {
            if (_reflectionBinder == null)
            {
                _reflectionBinder = new InjectorReflectionBinder();
            }
            if (_poolBinder == null)
            {
                _poolBinder = new PoolBinder();
            }

            Factory = _factory;
            Reflector = _reflectionBinder;
            PoolBinder = _poolBinder;
        }

        public object Instantiate(IInjectionBinding binding)
        {
            failIf(Binder == null, "Attempt to instantiate from Injector without a Binder", InjectionExceptionType.NO_BINDER);
            failIf(Factory == null, "Attempt to inject into Injector without a Factory", InjectionExceptionType.NO_FACTORY);

            object retVal = null;
            Type reflectionType = null;

            var bindingValue = binding.Value;
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
                failIf(reflection.ConstructorParameterCount > 0, "Attempt to inject a class with an only parameters constructor", InjectionExceptionType.NOEMPTY_CONSTRUCTOR);

                if (binding.Type == InjectionBindingType.POOL)
                {
                    var pool = PoolBinder.Get(reflectionType);
                    failIf(pool == null, "Attempt to instantiate a class with a null pool", InjectionExceptionType.NOEMPTY_CONSTRUCTOR);
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

            failIf(retVal == null, "the instantiate result is null", InjectionExceptionType.NULL_INSTANTIATE, binding.Key as Type, null);

            return retVal;
        }

        public object Inject(object target)
        {
            failIf(Binder == null, "Attempt to inject into Injector without a Binder", InjectionExceptionType.NO_BINDER);
            failIf(Reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
            failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

            //Some things can't be injected into. Bail out.
            Type t = target.GetType();
            if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
            {
                return target;
            }

            var reflection = Reflector.Get(t);
            // Actually we dont support construct inject
            failIf(reflection == null, "Attempt to perform constructor injection without a reflection", InjectionExceptionType.NULL_REFLECTION);
            failIf(reflection.Constructor == null, "Attempt to construction inject a null constructor", InjectionExceptionType.NULL_CONSTRUCTOR);
            failIf(reflection.ConstructorParameterCount > 0, "Attempt to construction inject a constructor-with parameters", InjectionExceptionType.NOEMPTY_CONSTRUCTOR);

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
            failIf(Binder == null, "Attempt to inject into Injector without a Binder", InjectionExceptionType.NO_BINDER);
            failIf(Reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
            failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

            Type t = target.GetType();
            if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
            {
                return;
            }

            IReflectedClass reflection = Reflector.Get(t);

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
            failIf(target == null, "Attempt to PostConstruct a null target", InjectionExceptionType.NULL_TARGET);
            failIf(reflection == null, "Attempt to PostConstruct without a reflection", InjectionExceptionType.NULL_REFLECTION);

            if (reflection.PostConstructor != null)
                reflection.PostConstructor.Invoke(target, null);
        }

        private void PerformFieldInjection(object target, IReflectedClass reflection)
        {
            failIf(target == null, "Attempt to inject into a null object", InjectionExceptionType.NULL_TARGET);
            failIf(reflection == null, "Attempt to inject without a reflection", InjectionExceptionType.NULL_REFLECTION);

            for (int i = 0; i < reflection.Fields.Length; i++)
            {
                var pair = reflection.Fields[i];
                var value = GetValueInjection(pair.Item1, pair.Item2, target);
                InjectValueIntoPoint(value, target, pair.Item3);
            }
        }

        //Inject the value into the target at the specified injection point
        private void InjectValueIntoPoint(object value, object target, FieldInfo point)
        {
            failIf(target == null, "Attempt to inject into a null target", InjectionExceptionType.NULL_TARGET);
            failIf(point == null, "Attempt to inject into a null point", InjectionExceptionType.NULL_INJECTION_POINT);
            failIf(value == null, "Attempt to inject null into a target object", InjectionExceptionType.NULL_VALUE_INJECTION);

            point.SetValue(target, value);
        }

        private void failIf(bool condition, string message, InjectionExceptionType type)
        {
            failIf(condition, message, type, null, null, null);
        }

        private void failIf(bool condition, string message, InjectionExceptionType type, Type t, object name)
        {
            failIf(condition, message, type, t, name, null);
        }

        private void failIf(bool condition, string message, InjectionExceptionType type, Type t, object name, object target)
        {
            if (condition)
            {
                message += "\n\t\ttarget: " + target;
                message += "\n\t\ttype: " + t;
                message += "\n\t\tname: " + name;
                throw new InjectionException(message, type);
            }
        }

        private object GetValueInjection(Type t, object name, object target)
        {
            var binding = Binder.GetBinding(t, name);
            failIf(binding == null, "Attempt to Instantiate a null binding.", InjectionExceptionType.NULL_BINDING, t, name, target);
            var bindingValue = binding.Value;

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
                retVal = binding.Value;
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