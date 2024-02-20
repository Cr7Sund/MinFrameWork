/**
 * @interface Cr7Sund.Framework.Api.IInjectionBinder
 *
 * A Binder that implements Dependency Injection .
 *
 * Keys in this Binder are always Types, that is, they represent
 * either Classes or Interfaces, not values. Values may be either Types
 * or values, depending on the situation.
 *
 * The nature of the instance returned by `GetInstance()` depends on how
 * that Key was mapped.
 *
 * examples:
 *
 * //Returns a new instance of SimpleInterfaceImplementer.
 *
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();`
 *
 * //Returns a Singleton instance of SimpleInterfaceImplementer.
 *
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToSingleton();`
 *
 * //Returns a Singleton instance of SimpleInterfaceImplementer.
 *
 * `Bind<ISimpleInterface>().ToValue(new SimpleInterfaceImplementer());`
 *
 * //Returns a named instance of SimpleInterfaceImplementer for those whose
 * //injections specify this name. Note that once requested, this
 * //same instance will be returned on any future request for that named instance.
 *
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>(SomeEnum.MY_ENUM);`
 *
 * //Raises an Exception. string does not Implement ISimpleInterface.
 *
 * `Bind<ISimpleInterface>().To<string>();`
 *
 * @see Cr7Sund.Framework.Api.IInjectionBinding
 */

using System;
using System.Collections.Generic;
namespace Cr7Sund.Package.Api
{
    public interface IInjectionBinder : IBinder, IInstanceProvider
    {
        ///<summary> Get or set an Injector to use. By default, Injector instantiates it's own, but that can be overridden.</summary>
        IInjector Injector { get; set; }

        /// <summary>
        ///     Retrieve an Instance based on a key/name combo.
        ///     ex. `injectionBinder.Get<cISomeInterface>(SomeEnum.MY_ENUM);
        /// </summary>
        T GetInstance<T>(object name);

        /// <summary>
        ///     Retrieve an Instance based on a key/name combo.
        ///     ex. `injectionBinder.Get(typeof(ISomeInterface), SomeEnum.MY_ENUM);`
        /// </summary>
        object GetInstance(Type key, object name);

        /// <summary>
        ///     Release an Instance based on a instance/name combo.
        /// </summary>
        void ReleaseInstance(object instance, object name = null);

        /// <summary>
        ///     Reflect all the types in the list
        ///     Return the number of types in the list, which should be equal to the list length
        /// </summary>
        int ReflectAll();

        /// <summary>
        ///     Reflect all the types currently registered with InjectionBinder
        ///     Return the number of types reflected, which should be equal to the number
        ///     of concrete classes you've mapped.
        /// </summary>
        int Reflect(List<Type> list);


        new IInjectionBinding Bind<T>();
        new IInjectionBinding GetBinding<T>(object name = null);
        IInjectionBinding GetBinding(Type key);
        IInjectionBinding GetBinding(Type key, object name);
    }
}
