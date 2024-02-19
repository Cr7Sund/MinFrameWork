using System;
namespace Cr7Sund.PackageTest.Api
{
    /// <summary>
    ///     Interface for a promise that can be canceled
    /// </summary>
    public interface ICancelable
    {
        /// <summary>
        ///     Cancel the promise 
        /// </summary>
        void Cancel();
    }
}
