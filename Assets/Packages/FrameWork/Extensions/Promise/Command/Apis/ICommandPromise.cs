using System;
using System.Collections.Generic;

namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents a command promise interface that extends IPromise, ISequence, IPoolable, and IResetable.
    /// </summary>
    public interface ICommandPromise<PromisedT> : IPromise<PromisedT>, ISequence, IPoolable, IResetable
    {
        Action<PromisedT> ExecuteHandler { get; }
        Action<float> SequenceProgressHandler { get; }
        Action<float> CommandProgressHandler { get; }
        Action<Exception> RejectHandler { get; }
        Action<float> ProgressHandler { get; }
        
        /// <summary>
        /// Chains a new command provided by type.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <returns>The new command promise.</returns>
        ICommandPromise<PromisedT> Then<T>() where T : ICommand<PromisedT>, new();

        /// <summary>
        /// Chains a new command provided by specific value.
        /// </summary>
        /// <param name="resultPromise">Result promise.</param>
        /// <param name="command">Command to chain.</param>
        /// <returns>The new command promise.</returns>
        ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, ICommand<PromisedT> command);

        /// <summary>
        /// Chains a new command of a new type provided by specific type.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <typeparam name="ConvertedT">Type to convert to.</typeparam>
        /// <returns>The new command promise.</returns>
        ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommand<PromisedT, ConvertedT>, new();

        /// <summary>
        /// Chains a new command of a new type provided by specific value.
        /// </summary>
        /// <param name="resultPromise">Result promise.</param>
        /// <param name="command">Command to chain.</param>
        /// <typeparam name="ConvertedT">Type to convert to.</typeparam>
        /// <returns>The new command promise.</returns>
        ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise, ICommand<PromisedT, ConvertedT> command);

        /// <summary>
        /// Chains an enumerable of promises and commands, all of which must resolve.
        /// Returns a promise for a collection of the resolved results.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);

        /// <summary>
        /// Chains an enumerable of promises and commands.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// It is rejected until all of the promises have been rejected.
        /// </summary>
        ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);

        /// <summary>
        /// Chains an enumerable of promises and commands.
        /// Returns a promise that resolves when any of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);

        /// <summary>
        /// Chains an enumerable of promises and commands.
        /// Returns a promise that resolves when the first of the promises or commands has resolved.
        /// Yields the value from the first promise or command that has resolved.
        /// It is rejected as soon as any of the promises or commands have been rejected.
        /// </summary>
        ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);

        /// <summary>
        /// Executes the command promise with a given value.
        /// </summary>
        /// <param name="value">The value to execute the command with.</param>
        void Execute(PromisedT value);

        /// <summary>
        /// Reports progress for the command promise.
        /// </summary>
        /// <param name="progress">The progress value.</param>
        void Progress(float progress);

        /// <summary>
        /// Handles exceptions for the command promise.
        /// </summary>
        /// <param name="e">The exception to handle.</param>
        void Catch(Exception e);
    }
}
