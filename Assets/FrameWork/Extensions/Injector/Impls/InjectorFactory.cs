using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class InjectorFactory : IInjectorFactory
    {
        public object Get(IInjectionBinding binding)
        {
            return this.Get(binding, null);
        }

        public object Get(IInjectionBinding binding, object[] args)
        {
            var type = binding.Type;

            var bindingValue = binding.Value;
            if (bindingValue == null)
            {
                throw new InjectionException("InjectorFactory cannot act on null binding", InjectionExceptionType.NULL_BINDING);
            }

            switch (type)
            {
                case InjectionBindingType.SINGLETON:
                    return SingletonOf(binding, args);
                case InjectionBindingType.VALUE:
                    return ValueOf(binding);
                default:
                    break;
            }

            return InstanceOf(binding, args);
        }

        private object InstanceOf(IInjectionBinding binding, object[] args)
        {
            return CreateFromValue(binding.Value, args);
        }

        private object ValueOf(IInjectionBinding binding)
        {
            var bindingValue = binding.Value;

            if (bindingValue.GetType().IsInstanceOfType(typeof(Type)))
                throw new InjectionException("Inject a type into binder as value", InjectionExceptionType.NULL_VALUE_INJECTION);

            return bindingValue;
        }

        private object SingletonOf(IInjectionBinding binding, object[] args)
        {
            var bindingValue = binding.Value;

            if (binding.Value != null)
            {
                if (bindingValue.GetType().IsInstanceOfType(typeof(Type)))
                {
                    object o = CreateFromValue(bindingValue, args);
                    if (o == null)
                    {
                        return null;
                    }

                    binding.SetValue(o);
                }
                else
                {
                    //no-op. We already have a binding value!
                }
            }
            else
            {
                throw new InjectionException("InjectorFactory cant instantiate a binding with value", InjectionExceptionType.NULL_VALUE_INJECTION);
            }

            return binding.Value;
        }

        /// Call the Activator to attempt instantiation the given object
        private object CreateFromValue(object o, object[] args)
        {
            var value = (o is Type) ? o as Type : o.GetType();

            object retVal = null;
            try
            {
                if (args == null || args.Length == 0)
                {
                    retVal = Activator.CreateInstance(value);
                }
                else
                {
                    retVal = Activator.CreateInstance(value, args);
                }
            }
            catch
            {
                //No-op
            }

            return retVal;
        }
    }
}