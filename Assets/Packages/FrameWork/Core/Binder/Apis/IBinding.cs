/**
 * @interface Cr7Sund.Framework.Api.IBinding
 *
 * Binds a key (type) to a value Semibinding.
 *
 * Bindings represent the smallest element with which most
 * developers will normally interact.
 *
 * A binding is made up of two required parts and one optional part .
 * <ul>
 * <li>key - The Type  that a client provides in order to unlock a value.</li>
 * <li>value - One or more things tied to and released by the offering of a key</li>
 * <li>name - An optional discriminator, allowing a client to differentiate between multiple keys of the same Type</li>
 * </ul>
 *
 * The required parts are a key and a value. The key Trigers the value;
 * thus an event can be the key that Trigers a callback.
 * Or the instantiation of one class can be the key that leads to the
 * instantiation of another class. The optional part is a name.
 * Under some circumstances, it is useful to qualify two bindings with identical keys.
 * Under these circumstances, the name serves as a discriminator.
 * <br />
 * <br />
 * Note that SemiBindings maintain lists, so RemoveKey, RemoveValue and RemoveName delete an entry from those lists.
 */
using System;

namespace Cr7Sund.Package.Api
{
    public interface IBinding : IDisposable
    {

        ///<summary> Get the binding's key </summary>
        ISemiBinding Key { get; }
        object Name { get; }
        ISemiBinding Value { get; }

        BindingConstraintType KeyConstraint { get; }

        /// <summary> Get whether or not the binding is weak, default false </summary>
        bool IsWeak { get; }
        /// <summary>
        ///     Tie this binding to a Type key
        /// </summary>
        IBinding Bind<T>();
        /// <summary>
        ///     Tie this binding to a key, such as object, string, enum
        /// </summary>
        IBinding Bind(object type);
        /// <summary>
        ///     Set the Binding's value to a value, such as a string or class instance
        /// </summary>
        IBinding To(object value);
        /// <summary>
        ///     Set the Binding's value to a Type
        /// </summary>
        IBinding To<T>();
        /// <summary>
        ///     Qualify a binding using a value, such as a string or enum
        ///     should be call first otherwise maybe cause an conflict BinderException
        /// </summary>
        IBinding ToName(object name);

        //<summary> Remove a specific value from the binding </summary>
        void RemoveValue(object o);

        /// <summary> Mark a binding as weak, so that any new binding will override it </summary>
        IBinding Weak();
    }

    public enum BindingConstraintType
    {
        /// Constrains a SemiBinding to carry no more than one item in its Value
        ONE,
        /// Constrains a SemiBinding to carry a list of items in its Value
        MANY,
        /// Instructs the Binding to apply a Pool instead of a SemiBinding
        POOL
    }
}
