/**
 * @interface Cr7Sund.Framework.Api.IPoolable
 *
 * Interface for items that belong to a Pool
 *
 * Instances that belong to a Pool usually need to be cleaned up for later
 * reuse. This is both an aspect of careful instance wrangling and of memory management,
 * since you want to release any memory references possibly attached to the poolable
 * items. If your instances implement IPoolable, place all cleanup code inside Restore(),
 * and the cleanup will occur automatically when the instance is returned to the Pool.
 *
 * If you cannot or do not wish to implement IPoolable, that's entirely allowable, but
 * know that it will be your responsibility to clean up an instance BEFORE returning
 * it to the Pool.
 */

namespace Cr7Sund.Framework.Api
{
    public interface IPoolable
    {
        /// <summary>
        ///     Is this instance retained?
        /// </summary>
        /// <value><c>true</c> if retained; otherwise, <c>false</c>.</value>
        bool IsRetain { get; }
        /// <summary>
        ///     Clean up this instance for reuse.
        /// </summary>
        /// Restore methods should clean up the instance sufficiently to remove prior state.
        void Restore();

        /// <summary>
        ///     Keep this instance from being returned to the pool
        /// </summary>
        void Retain();

        /// <summary>
        ///     Release this instance back to the pool.
        /// </summary>
        /// Release methods should clean up the instance sufficiently to remove prior state.
        void Release();
    }
}
