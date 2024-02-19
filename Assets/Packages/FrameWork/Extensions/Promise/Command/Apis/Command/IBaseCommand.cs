using System;

namespace Cr7Sund.PackageTest.Api
{
    /// <summary>
    /// Represents the base interface for commands, providing methods to handle exceptions and report progress.
    /// </summary>
    public interface IBaseCommand
    {
        /// <summary>
        /// Handles an exception that occurred during command execution.
        /// </summary>
        /// <param name="e">The exception to handle.</param>
        void OnCatch(Exception e);

        /// <summary>
        /// Reports progress during command execution.
        /// </summary>
        /// <param name="progress">The progress value to report.</param>
        void OnProgress(float progress);
    }
}
