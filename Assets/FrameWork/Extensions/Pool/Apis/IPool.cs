/**
 * @interface Cr7Sund.Framework.Api.IPool
 *
 * A mechanism for storing and reusing instances.
 *
 * you can employ Pools yourself by mapping and injecting a Pool for instances you want to reuse.
 *
 * Basic instructions for injecting a Pool for use:
 * Map IPool<SomeClass> in the InjectionBinder:

        injectionBinder.Bind<IPool<MyClass>>().ToSingleton();

* Then inject like so:

        [Inject]
        public IPool<MyClass> myPool { get; set; }
 *
 * A couple of caveats for working with Pools:
 * 1. A limitation of the version of .NET currently used by Unity forbids using interfaces or abstracts in generics.
 * so you cannot map and inject IPool<IMyInterface> or IPool<MyAbstractClass>. This is a little confusing in
 * Strange, since we're used to mapping injections in exactly this fashion (e.g., injectionBinder.Bind<ISomeInterface>).
 * The reason this doesn't work for Pools has to do with setting properties, rather than the binding itself.
 * But because it will bite you, we throw an Exception if you attempt to Bind or set anything but a concrete Pool type.
 *
 * 2. Pooling presupposes that when the instance is finished doing what it does it is cleaned up and
 * returned to the Pool. Use IPool.ReturnInstance() to mark an object as ready for reuse.
 * @see Cr7Sund.Framework.Api.IPoolable for more on cleaning up.
 */


using System;
namespace Cr7Sund.Framework.Api
{
    public interface IPool<T> : IBasePool, IManagedList
    {
        /// <summary>
        ///     Gets an instance from the pool if one is available.
        /// </summary>
        /// <returns>The instance.</returns>
        T GetInstance();

        /// <summary>
        ///     Returns an instance to the pool.
        /// </summary>
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// <param name="value">The generic instance to be return to the pool.</param>
        void ReturnInstance(T value);

        /// <summary>
        ///     Returns an instance to the pool.
        ///     will do conversion nextly, so don't pass the value type object
        /// </summary>
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// <param name="value">The instance to be return to the pool.</param>
        void ReturnInstance(object value);
    }


    public interface IPool : IBasePool, IManagedList
    {
        /// <summary>
        ///     The object Type of the first object added to the pool.
        ///     Pool objects must be of the same concrete type. This property enforces that requirement.
        /// </summary>
        Type PoolType { get; set; }

        /// <summary>
        ///     Gets an instance from the pool if one is available.
        /// </summary>
        /// <returns>The instance.</returns>
        object GetInstance();

        /// <summary>
        ///     Returns an instance to the pool.
        /// </summary>
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// <param name="value">The instance to be return to the pool.</param>
        void ReturnInstance(object value);
    }


}
