


using System;

/**
 * @interface Cr7Sund.Framework.Api.ISemiBinding
 *
 * A managed list of values.
 *
 * A SemiBinding is the smallest atomic part of the strange framework. It represents
 * either the Value or the Name arm of the binding.
 * <br />
 * The SemiBinding stores some value...a system Type, a list, a concrete value.
 * <br />
 * It also has a constraint defined by the constant ONE or MANY.
 * A constraint of ONE makes the SemiBinding maintain a singular value, rather than a list.
 * <br />
 * The default constraints are:
 * <ul>
 *  <li>Key - ONE</li>
 *  <li>Value - MANY</li>
 *  <li>Name - ONE</li>
 * </ul>
 *
 * @see Cr7Sund.Framework.Api.BindingConstraintType
 */
namespace Cr7Sund.PackageTest.Api
{
    public interface ISemiBinding : IManagedList
    {
        /// <summary>  Set or get the constraint.  </summary>
        BindingConstraintType Constraint { get; set; }

        /// <summary>
        ///     A secondary constraint that ensures that this SemiBinding will never contain multiple values equivalent to
        ///     each other.
        /// </summary>
        bool UniqueValue { get; set; }
        /// <summary>
        ///     Gets or sets the type of inflation for infinite-sized pools.
        /// </summary>
        /// By default, a pool doubles its InstanceCount.
        /// <value>A PoolInflationType value.</value>
        PoolInflationType InflationType { get; set; }

        object SingleValue{get;}

        object this[int index] { get; set; }


        object[] Clone();
    }


}
