using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class InjectorFactory : IInjectorFactory
    {
        public object Get(IInjectionBinding binding)
        {
            return Get(binding, null);
        }

        public object Get(IInjectionBinding binding, object[] args)
        {
            var type = binding.Type;

            object bindingValue = binding.Value;
            AssertUtil.NotNull(bindingValue, InjectionExceptionType.GET_NULL_BINDING_FACTORY);

            switch (type)
            {
                case InjectionBindingType.SINGLETON:
                    return SingletonOf(binding, args);
                case InjectionBindingType.VALUE:
                    return ValueOf(binding);
            }

            return InstanceOf(binding, args);
        }

        private object InstanceOf(IInjectionBinding binding, object[] args)
        {
            return CreateFromValue(binding.Value, args);
        }

        private object ValueOf(IInjectionBinding binding)
        {
            object bindingValue = binding.Value;
   
            if (bindingValue.GetType().IsInstanceOfType(typeof(Type)))
                throw new MyException("Inject a type into binder as value", InjectionExceptionType.TYPE_AS_VALUE_INJECTION);

            return bindingValue;
        }

        private object SingletonOf(IInjectionBinding binding, object[] args)
        {
            object bindingValue = binding.Value;

            AssertUtil.NotNull(bindingValue, InjectionExceptionType.EXISTED_VALUE_INJECTION);

            if (bindingValue.GetType().IsInstanceOfType(typeof(Type)))
            {
                object o = CreateFromValue(bindingValue, args);
                if (o == null)
                {
                    return null;
                }

                binding.SetValue(o);
            }

            return binding.Value;
        }

        /// Call the Activator to attempt instantiation the given object
        private object CreateFromValue(object o, object[] args)
        {
            var value = o is Type ? o as Type : o.GetType();

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
