using System;
namespace Cr7Sund.Package.Api
{
    /// <summary>
    ///     Interface for a promise that can be rejected.
    /// </summary>
    public interface IRejectable
    {
        /// <summary>
        ///     Reject the promise with an exception.
        /// </summary>
        void Reject(Exception ex);

        /// <summary>
        ///     Reject the promise with an exception without log
        /// </summary>
        void RejectWithoutDebug(Exception ex);
    }
}
