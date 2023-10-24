using System;

namespace Cr7Sund.Framework.Api
{
    public interface IInjectionBinding : IBinding
    {
        /// Map the Binding to a Singleton so that every `GetInstance()` on the Binder Key returns the same instance.
        IInjectionBinding AsSingleton();

        /// Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided instance. Sets type to Default
        IInjectionBinding AsNew();

        /// Map the binding and give access to all contexts in hierarchy
        IInjectionBinding CrossContext();

        bool IsCrossContext { get; }

        /// Boolean setter to optionally override injection. If false, the instance will not be injected after instantiation.
        IInjectionBinding ToInject(bool value);

        /// Get the parameter that specifies whether this Binding allows an instance to be injected
        bool IsToInject { get; }

        /// Get and set the InjectionBindingType
        /// @see InjectionBindingType
        InjectionBindingType Type { get; set; }


        new IInjectionBinding Bind<T>();
        IInjectionBinding Bind(Type type);
        new IInjectionBinding To<T>();
        IInjectionBinding To(Type type);
        /// Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided imstance. Sets type to Value
        IInjectionBinding ToValue(object o );
        /// Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided imstance. Does not set type.
        IInjectionBinding SetValue(object o);
        new IInjectionBinding ToName(object name);
        new IInjectionBinding Weak();
    }

    public enum InjectionBindingType
    {
        /// The binding provides a new instance every time
        DEFAULT,

        /// The binding always provides the same instance
        SINGLETON,

        /// The binding always provides the same instance based on a provided value
        VALUE,
    }
}

