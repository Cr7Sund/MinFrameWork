namespace Cr7Sund.Framework.Api
{
    public enum InjectionExceptionType
    {
        /// No InjectionBinder found.
        NO_BINDER,
        /// Attempt to instantiate from Injector without a Binder
        NO_BINDER_INSTANTIATE,
        // Attempt to inject into Injector without a Binder
        NO_BINDER_INJECT,
        // Attempt to uninject from Injector without a Binder
        NO_BINDER_UnINJECT,
        /// No ReflectionBinder found when inject
        NO_REFLECTOR,
        /// No ReflectionBinder found when uninject
        NO_REFLECTOR_UNINJECT,
        /// No InjectorFactory found.
        NO_FACTORY,
        // Attempt to inject into Injector without a Factory
        NO_FACTORY_INSTANTIATE,
        // Attempt to inject into Injector without a Factory
        NO_FACTORY_INJECT,
        /// The requested Binding was null or couldn't be found when create
        NULL_BINDING_CREATE,
        /// The requested Binding was null or couldn't be found when release
        NULL_BINDING_RELEASE,
        /// The requested Binding was null or couldn't be found when GetValueInjection
        NULL_BINDING_GET_INJECT,
        // try to release an null instance
        // e.g.  
        // private void AssignNull(ref GuaranteedUniqueInstances instances)
        // {
        //     instances = null;
        // }
        // AssignNull(ref instance1);
        // binder.ReleaseInstance(instance1, SomeEnum.ONE);
        RELEASE_NULL_BINDING,
        // InjectorFactory cannot act on null binding
        GET_NULL_BINDING_FACTORY,
        /// No reflection was provided for the requested class.
        NULL_REFLECTION,
        /// Attempt to inject without a reflection
        NULL_REFLECTION_FIELDINJECT,
        /// Attempt to perform constructor injection without a reflection
        NULL_REFLECTION_INSTANTIATE,
        ///  "Attempt to PostConstruct without a reflection",
        NULL_REFLECTION_POSTINJECT,
        /// The instance being injected into resolved to null.
        NULL_TARGET,
        /// Attempt to inject into a null object
        NULL_TARGET_FIELDINJECT,
        /// Attempt to inject into null instance
        NULL_TARGET_INJECT,
        /// Attempt to uninject into null instance
        NULL_TARGET_UNINJECT,
        /// Attempt to postinject into null instance
        NULL_TARGET_POSTINJECT,
        /// During an attempt to construct, constructors except primitive types was not allowed
        NONEMPTY_CONSTRUCTOR,
        // "Attempt to instantiate a class with a null pool"
        NOPOOL_CONSTRUCT,
        /// The value being injected into the target resolved to null.
        NULL_VALUE_INJECTION,
        //InjectorFactory cant instantiate a binding with value
        EXISTED_VALUE_INJECTION,
        // Inject a type into binder as value
        TYPE_AS_VALUE_INJECTION,
        /// During setter injection the requested setter resolved to null.
        NULL_INJECTION_POINT,
        /// During an attempt to construct, no constructor was found.
        NULL_CONSTRUCTOR,

        // the instantiate result is null which is forbidden
        NULL_INSTANTIATE,
        // the instantiate result is null
        NULL_INSTANTIATE_RESULT,
        /// The value of a binding does not extend or implement the binding type.
        ILLEGAL_BINDING_VALUE,
        /// During an attempt to construct, no pool can be instantiate
        NULL_POOL,
        // try to release a null instance, pay attention to that. maybe you have destroy the instance at first
        NULL_RELEASE,
        // ioc dependency circle when inject field value 
        INJECT_DEPTH_LIMIT
    }
}
