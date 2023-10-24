using System;

namespace Cr7Sund.Framework.Api
{
    public interface IInjectionBinding : IBinding
    {
        /// <summary>  Map the Binding to a Singleton so that every `GetInstance()` on the Binder Key returns the same instance. </summary> 
        IInjectionBinding AsSingleton();

        /// <summary>  Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided instance. Sets type to Default </summary> 
        IInjectionBinding AsNew();

        /// <summary>  Map the binding and give access to all contexts in hierarchy </summary> 
        IInjectionBinding CrossContext();

        bool IsCrossContext { get; }

        /// <summary>  Boolean setter to optionally override injection. If false, the instance will not be injected after instantiation. </summary> 
        IInjectionBinding ToInject(bool value);

        /// <summary>  Get the parameter that specifies whether this Binding allows an instance to be injected </summary> 
        bool IsToInject { get; }

        /// <summary>  
        /// Get and set the InjectionBindingType 
        /// @see InjectionBindingType 
        /// </summary> 
        InjectionBindingType Type { get; set; }


        new IInjectionBinding Bind<T>();
        IInjectionBinding Bind(Type type);
        new IInjectionBinding To<T>();
        IInjectionBinding To(Type type);
        /// <summary>  Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided instance. Sets type to Value </summary> 
        IInjectionBinding ToValue(object o);
        /// <summary>  Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided instance. Does not set type. </summary> 
        IInjectionBinding SetValue(object o);
        new IInjectionBinding ToName(object name);
        new IInjectionBinding Weak();
    }

    public enum InjectionBindingType
    {
        /// <summary>  The binding provides a new instance every time </summary> 
        DEFAULT,

        /// <summary>  The binding always provides the same instance </summary> 
        SINGLETON,

        /// <summary>  The binding always provides the same instance based on a provided value </summary> 
        VALUE,
    }
}

