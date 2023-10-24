using System;

namespace Cr7Sund.Framework.Api
{
    public enum InjectionExceptionType
    {
        /// No InjectionBinder found.
        NO_BINDER,
        /// No ReflectionBinder found.
        NO_REFLECTOR,
        /// No InjectorFactory found.
        NO_FACTORY,
        /// The requested Binding was null or couldn't be found.
        NULL_BINDING,
        /// No reflection was provided for the requested class.
        NULL_REFLECTION,
        /// The instance being injected into resolved to null.
        NULL_TARGET,
        /// During an attempt to construct, constructors except primitive types was not allowed
        NOEMPTY_CONSTRUCTOR,
        /// The value being injected into the target resolved to null.
        NULL_VALUE_INJECTION,
        /// During setter injection the requested setter resolved to null.
        NULL_INJECTION_POINT,
        /// During an attempt to construct, no constructor was found.
        NULL_CONSTRUCTOR,
        // the instantiate result is null which is forbidden
        NULL_INSTANTIATE,
        /// The value of a binding does not extend or implement the binding type.
        ILLEGAL_BINDING_VALUE,
    }
}