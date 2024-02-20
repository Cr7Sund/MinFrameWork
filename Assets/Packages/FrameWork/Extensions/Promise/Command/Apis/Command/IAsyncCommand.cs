using System;

namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents an asynchronous command without parameters or return value.
    /// </summary>
    public interface IAsyncCommand : IBaseCommand
    {
        /// <summary>
        /// Executes the asynchronous command and returns a promise.
        /// </summary>
        /// <returns>A promise representing the asynchronous execution.</returns>
        IPromise OnExecuteAsync();

        /// <summary>
        /// Handles an exception asynchronously that occurred during command execution.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <returns>A promise representing the asynchronous exception handling.</returns>
        IPromise OnCatchAsync(Exception ex);
    }

    /// <summary>
    /// Represents an asynchronous command with a promised parameter and return value.
    /// </summary>
    /// <typeparam name="PromisedT">The type of the promised parameter and return value.</typeparam>
    public interface IAsyncCommand<PromisedT> : IBaseCommand
    {
        /// <summary>
        /// Executes the asynchronous command with a promised parameter and returns a promise.
        /// </summary>
        /// <param name="value">The promised parameter value.</param>
        /// <returns>A promise representing the asynchronous execution with a promised return value.</returns>
        IPromise<PromisedT> OnExecuteAsync(PromisedT value);

        /// <summary>
        /// Handles an exception asynchronously that occurred during command execution.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <returns>A promise representing the asynchronous exception handling.</returns>
        IPromise<PromisedT> OnCatchAsync(Exception ex);
    }

    /// <summary>
    /// Represents an asynchronous command with a promised input parameter and a converted return value.
    /// </summary>
    /// <typeparam name="PromisedT">The type of the promised input parameter.</typeparam>
    /// <typeparam name="ConvertedT">The type of the converted return value.</typeparam>
    public interface IPromiseAsyncCommand<in PromisedT, ConvertedT> : IBaseCommand
    {
        /// <summary>
        /// Executes the asynchronous command with a promised input parameter and returns a promise with a converted return value.
        /// </summary>
        /// <param name="value">The promised input parameter value.</param>
        /// <returns>A promise representing the asynchronous execution with a converted return value.</returns>
        IPromise<ConvertedT> OnExecuteAsync(PromisedT value);

        /// <summary>
        /// Handles an exception asynchronously that occurred during command execution.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <returns>A promise representing the asynchronous exception handling.</returns>
        IPromise<ConvertedT> OnCatchAsync(Exception ex);
    }
}
