/**
 * @interface Cr7Sund.Framework.Api.IBinder
 *
 * Collection class for bindings.
 *
 * Binders are a collection class (akin to ArrayList and Dictionary)
 * with the specific purpose of connecting lists of things that are
 * not necessarily related, but need some type of runtime association.
 *
 * Think of each Binder as a collection of causes and effects, or actions
 * and reactions. If the Key action happens, it Trigers the Value
 * action. So, for example, an Event may be the Key that Trigers
 * instantiation of a particular class.
 *
 * @see Cr7Sund.Framework.Api.IBinding
 */
using System;
namespace Cr7Sund.Package.Api
{
    public interface IBinder : IDisposable
    {
        /// <summary>
        ///     Bind a Binding Key to a class or interface generic
        /// </summary>
        IBinding Bind<T>();
        /// <summary>
        ///     Bind a Binding Key to a object
        /// </summary>
        IBinding Bind(object key);

        /// <summary>
        ///     Retrieve a binding based on the provided Type
        /// </summary>
        IBinding GetBinding<T>(object name = null);
        /// <summary>
        ///     Retrieve a binding based on the provided key
        /// </summary>
        IBinding GetBinding(object key);

        /// <summary>
        ///     Retrieve a binding based on the provided key
        /// </summary>
        IBinding GetBinding(object key, object name);

        /// <summary>
        ///     Remove a binding based on the provided Key (generic)
        /// </summary>
        void Unbind<T>(object name = null);

        /// <summary>
        ///     Remove a binding based on the provided Key (generic)
        /// </summary>
        void Unbind(object key);

        /// <summary> Remove the provided binding from the Binder </summary>
        void Unbind(IBinding binding);

        /// <summary>
        ///     Remove a binding based on the provided Key (generic)
        /// </summary>
        void Unbind(object key, object name);

        /// <summary> Remove a select value from the given binding </summary>
        void RemoveValue(IBinding binding, object value);

        /// <summary>
        ///     The Binder is being removed
        ///     Override this method to clean up remaining bindings
        ///     <summary>
        ///     </summary>
        void OnRemove();

        /// <summary>
        ///     Places individual Bindings into the bindings Dictionary as part of the resolving process
        /// </summary>
        void ResolveBinding(IBinding binding, object key, object oldName = null);

        /// <summary>
        /// copy bindings from binder
        /// </summary>
        /// <param name="fromBinder"></param>
        void CopyFrom(IBinder fromBinder);
        /// <summary>
        /// remove all  bindings 
        /// not unbind since the binding maybe come from other binders
        /// </summary>
        void RemoveAll();
    }
}
