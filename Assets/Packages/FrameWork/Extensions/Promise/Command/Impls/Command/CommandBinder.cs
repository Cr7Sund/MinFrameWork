using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class CommandBinder : Binder, ICommandBinder
    {
        T ICommandBinder.GetOrCreate<T>()
        {
            var binding = GetBinding<T>();
            if (binding == null)
            {
                var tInstance = Activator.CreateInstance<T>();
                binding = Bind<T>().To(tInstance);
            }

            var bindingValue = binding.Value.SingleValue as T;
            return bindingValue;
        }
        
        T ICommandBinder.Get<T>()
        {
            var binding = GetBinding<T>();
            if (binding != null)
            {
                var bindingValue = binding.Value.SingleValue as T;
                return bindingValue;
            }

            return null;
        }
    }
}
