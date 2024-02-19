using System;
namespace Cr7Sund.PackageTest.Api
{
    public interface IInjectionBinding : IBinding
    {

        bool IsCrossContext { get; }

        /// <summary>  Get the parameter that specifies whether this Binding allows an instance to be injected </summary>
        bool IsToInject { get; }

        /// <summary>
        ///     Get and set the InjectionBindingType
        ///     @see InjectionBindingType
        /// </summary>
        InjectionBindingType Type { get; set; }
        /// <summary>  Map the Binding to a Singleton so that every `GetInstance()` on the Binder Key returns the same instance. </summary>
        IInjectionBinding AsSingleton();

        /// <summary>
        ///     Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided
        ///     instance. Sets type to Default
        /// </summary>
        IInjectionBinding AsDefault();
        /// <summary>
        ///     Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided
        ///     instance. Sets type to Pool
        /// </summary>
        IInjectionBinding AsPool();

        /// <summary>  
        ///   Map the binding and give access to all contexts in hierarchy
        /// </summary>
        IInjectionBinding AsCrossContext();

        /// <summary>
        ///     Boolean setter to optionally override injection. If false, the instance will not be injected after
        ///     instantiation.
        /// </summary>
        IInjectionBinding ToInject(bool value);

        new IInjectionBinding Bind<T>();

        new IInjectionBinding To<T>();
        IInjectionBinding To(Type type);
        /// <summary>
        ///     Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided
        ///     instance. Sets type to Value
        /// </summary>
        new IInjectionBinding To(object o);

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
        /// <summary>  The binding provides a new instance every time from pool </summary>
        POOL
    }
}
