using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
{
    public class CrossContextInjectionBinder : InjectionBinder, ICrossContextInjectionBinder
    {
        public IInjectionBinder CrossContextBinder { get; set; }

        public override IInjectionBinding GetBinding(Type type, object name)
        {
            var binding = base.GetBinding(type, name);
            if (binding == null) // Attempt to get this from the cross context.Cross context is always SECOND PRIORITY.Local injections always override
            {
                if (CrossContextBinder != null)
                {
                    binding = CrossContextBinder.GetBinding(type, name);
                }
            }

            return binding;
        }

        public override void ResolveBinding(IBinding binding, object key, object oldName = null)
        {
            // Decide whether to resolve locally or not
            // If the binding is cross-context, we need to resolve it in cross-context
            // which also means the origin local binder will not contain the binding any more
            if (binding is IInjectionBinding injectionBinding)
            {
                if (injectionBinding.IsCrossContext)
                {
                    if (CrossContextBinder != null)
                    {
                        Unbind(key, binding.Name);
                        CrossContextBinder.ResolveBinding(binding, key, oldName);
                    }
                    else //We are a CrossContextBinder
                    {
                        base.ResolveBinding(binding, key, oldName);
                    }
                }
                else
                {
                    base.ResolveBinding(binding, key, oldName);
                }
            }
        }

        protected override IInjector GetInjectorForBinding(IInjectionBinding binding)
        {
            if (binding.IsCrossContext && CrossContextBinder != null)
            {
                return CrossContextBinder.Injector;
            }
            return base.GetInjectorForBinding(binding);
        }
    }
}
