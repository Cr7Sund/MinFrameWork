using System;
namespace Cr7Sund.Framework.Api
{
    public interface IBasePromise : IDisposable
    {
        /// <summary>
        ///     Complete the promise. Adds a default error handler.
        /// </summary>
        IDisposable Done();
    }
    
    public enum PromiseState
    {
        Pending, // The promise is in-flight.
        Rejected, // The promise has been rejected.
        Resolved // The promise has been resolved.
    }
}
