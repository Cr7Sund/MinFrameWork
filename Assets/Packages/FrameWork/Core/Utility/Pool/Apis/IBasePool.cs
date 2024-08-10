using System;

namespace Cr7Sund.FrameWork.Util
{
    /// <summary>
    /// Represents the base pool interface for storing and reusing instances.
    /// </summary>
    public interface IBasePool : IPoolable, IDisposable
    {
        /// <summary>
        /// Gets or sets the instance provider that provides instances to the pool.
        /// </summary>
        IInstanceProvider InstanceProvider { get; set; }

        /// <summary>
        /// Returns the count of non-committed instances.
        /// </summary>
        int Available { get; }
        /// <summary>
        /// Returns the count of all created instances.
        /// </summary>
        int TotalLength { get; }
        int Count { get; }

        /// <summary>
        /// Gets or sets the overflow behavior of this pool.
        /// </summary>
        /// <value>A PoolOverflowBehavior value.</value>
        PoolOverflowBehavior OverflowBehavior { get; set; }

        /// <summary>
        /// Gets or sets the type of inflation for infinite-sized pools.
        /// By default, a pool doubles its InstanceCount.
        /// </summary>
        /// <value>A PoolInflationType value.</value>
        PoolInflationType InflationType { get; set; }

        /// <summary>
        /// Gets or sets the max count of the pool in case of the pool increasing too fast with not returning instances.
        /// By default is 24, a lucky number.
        /// </summary>
        int MaxCount { get; set; }

        /// <summary>
        /// Sets the size of the pool.
        /// </summary>
        /// <param name="size">The pool size. '0' is a special value indicating infinite size. Infinite pools expand as necessary.</param>
        void SetSize(int size);
    }

    /// <summary>
    /// Represents the overflow behavior of a pool.
    /// </summary>
    public enum PoolOverflowBehavior
    {
        /// <summary>
        /// Requesting more than the fixed size will throw an exception.
        /// </summary>
        EXCEPTION,

        /// <summary>
        /// Requesting more than the fixed size will throw a warning.
        /// </summary>
        WARNING,

        /// <summary>
        /// Requesting more than the fixed size will return null and not throw an error.
        /// </summary>
        IGNORE
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, add one to the pool.
        INCREMENT,

        /// When a dynamic pool inflates, double the size of the pool
        DOUBLE
    }
}
