using System;

namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents a mechanism for storing and reusing instances.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
    public interface IPool<T> : IBasePool, IManagedList
    {
        /// <summary>
        /// Gets an instance from the pool if one is available.
        /// </summary>
        /// <returns>The instance.</returns>
        T GetInstance();

        /// <summary>
        /// Returns an instance to the pool.
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// </summary>
        /// <param name="value">The generic instance to be returned to the pool.</param>
        void ReturnInstance(T value);

        /// <summary>
        /// Returns an instance to the pool.
        /// Will do conversion nextly, so don't pass the value type object.
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// </summary>
        /// <param name="value">The instance to be returned to the pool.</param>
        void ReturnInstance(object value);
    }

    /// <summary>
    /// Represents a mechanism for storing and reusing instances.
    /// </summary>
    public interface IPool : IBasePool, IManagedList
    {
        /// <summary>
        /// Gets or sets the object Type of the first object added to the pool.
        /// Pool objects must be of the same concrete type. This property enforces that requirement.
        /// </summary>
        Type PoolType { get; set; }

        /// <summary>
        /// Gets an instance from the pool if one is available.
        /// </summary>
        /// <returns>The instance.</returns>
        object GetInstance();

        /// <summary>
        /// Returns an instance to the pool.
        /// If the instance being released implements IPoolable, the Release() method will be called.
        /// </summary>
        /// <param name="value">The instance to be returned to the pool.</param>
        void ReturnInstance(object value);
    }
}
