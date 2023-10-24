namespace Cr7Sund.Framework.Api
{
    public interface IInjector
    {
        /// <summary> 
        /// Request an instantiation based on the given binding.
        /// This request is made to the Injector, but it's really the InjectorFactory that does the instantiation.
        /// </summary> 
        object Instantiate(IInjectionBinding binding);

        /// <summary> 
        /// Request that the provided target be injected.
        /// </summary> 
        object Inject(object target);

        /// <summary> 
        /// Clear the injections from the provided instance.
        /// Note that Uninject can only clean public fields ...therefore only
        /// setters will be uninjected...not injections provided via constructor injection
        /// </summary> 
        void Uninject(object target);

        ///<summary>  Get/set an InjectorFactory.   </summary> 
        IInjectorFactory Factory { get; set; }

        /// <summary> Get/set an InjectionBinder. </summary> 
        IInjectionBinder Binder { get; set; }

        ///<summary>  Get/set a ReflectionBinder.</summary> 
        IReflectionBinder Reflector { get; set; }
    }
}