using System;

namespace Cr7Sund.FrameWork.Util
{
    /// <summary>
    /// Represents a binder for managing pools.
    /// </summary>
    public interface IPoolBinder : IDisposable
    {
        int Count { get; }


        /// <summary>
        /// Get a no-value pool based on the provided type.
        /// If not exist, it will return a newly created pool instead.
        /// </summary>
        IPool GetOrCreate(Type type);

        /// <summary>
        /// Get a value pool based on the provided type.
        /// If not exist, it will return a newly created pool instead.
        /// </summary>
        IPool<T> GetOrCreate<T>() where T : new();

        /// <summary>
        /// Get a value pool based on the provided type with a specified maximum pool count.
        /// If not exist, it will return a newly created pool instead.
        /// </summary>
        IPool<T> GetOrCreate<T>(int maxPoolCount) where T : new();

        /// <summary>
        /// Get a no-value pool based on the provided type.
        /// </summary>
        IPool Get(Type type);

        /// <summary>
        /// Get a value pool based on the provided type.
        /// If not exist, it will return a newly created pool instead.
        /// </summary>
        IPool<T> Get<T>() where T : new();

    }
}
