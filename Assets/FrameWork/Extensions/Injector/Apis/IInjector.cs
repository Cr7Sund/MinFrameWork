namespace Cr7Sund.Framework.Api
{
    public interface IInjector
    {
        /// Request an instantiation based on the given binding.
        /// This request is made to the Injector, but it's really the InjectorFactory that does the instantiation.
        object Instantiate(IInjectionBinding binding);


        /// Request that the provided target be injected.
        object Inject(object target);

        /// Clear the injections from the provided instance.
        /// Note that Uninject can only clean public fields ...therefore only
        /// setters will be uninjected...not injections provided via constructor injection
        void Uninject(object target);

        /// Get/set an InjectorFactory.
        IInjectorFactory Factory { get; set; }

        /// Get/set an InjectionBinder.
        IInjectionBinder Binder { get; set; }

        /// Get/set a ReflectionBinder.
        IReflectionBinder Reflector { get; set; }
    }
}