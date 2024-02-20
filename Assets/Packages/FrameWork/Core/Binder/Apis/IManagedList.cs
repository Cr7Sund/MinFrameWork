/**
 * @interface Cr7Sund.Framework.Api.IManagedList
 *
 * A common interface for the constituents parts of a Binding, which at present
 * are either a SemiBinding or a Pool. A ManagedList can have objects added or removed.
 */
using System;
namespace Cr7Sund.Package.Api
{
    public interface IManagedList : IDisposable
    {

        // Length of values
        int Count { get; }
        /// Add a value to this List.
        IManagedList Add(object value);

        /// Add a set of values to this List.
        IManagedList Add(object[] list);

        /// Remove a value from this List.
        IManagedList Remove(object value);

        /// Remove a set of values from this List.
        IManagedList Remove(object[] list);

        bool Contains(object o);
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, add one to the pool.
        INCREMENT,

        /// When a dynamic pool inflates, double the size of the pool
        DOUBLE
    }
}
