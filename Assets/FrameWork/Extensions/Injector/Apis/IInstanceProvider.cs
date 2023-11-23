/**
 * @interface Cr7Sund.Framework.Api.IInstanceProvider
 *
 * Provides an instance of the specified Type
 * When all you need is a new instance, use this instead of IInjectionBinder.
 */
using System;
namespace Cr7Sund.Framework.Api
{
    public interface IInstanceProvider
    {
        /// <summary>
        ///     Retrieve an Instance based on the key.
        ///     ex. `injectionBinder.Get<cISomeInterface>();`
        /// </summary>
        T GetInstance<T>();

        /// <summary>
        ///     Retrieve an Instance based on the key.
        ///     ex. `injectionBinder.Get(typeof(ISomeInterface));`
        /// </summary>
        object GetInstance(Type key);
    }
}
