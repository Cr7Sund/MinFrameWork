using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class CommandBinder : Binder, ICommandBinder
    {
        public T Get<T>() where T : class, IBaseCommand
        {
            var binding = this.GetBinding<T>();
            if (binding == null)
            {
                var tInstance = Activator.CreateInstance<T>();
                binding = Bind<T>().To(tInstance);
            }
            
            var bindingValue = binding.Value as T;
            bindingValue.Retain();
            return bindingValue;
        }
    }
}