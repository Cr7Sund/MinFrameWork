using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
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
