using System;
using System.Collections.Generic;

namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents a command promise, extending the generic IPromise interface with additional command-specific functionalities.
    /// </summary>
    public interface ICommandPromise : IPromise, ISequence, IPoolable, IResetable
    {
        /// <summary>
        /// Gets the handler for the reject action.
        /// </summary>
        Action<Exception> RejectHandler{get;}
        /// <summary>
        /// Gets the handler for the execute action.
        /// </summary>
        Action ExecuteHandler { get; }

        /// <summary>
        /// Gets the handler for the sequence progress action.
        /// </summary>
        Action<float> SequenceProgressHandler { get; }

        /// <summary>
        /// Gets the handler for the command progress action.
        /// </summary>
        Action<float> CommandProgressHandler { get; }

        /// <summary>
        /// Chains a new command provided by type.
        /// </summary>
        /// <typeparam name="T">The type of the command.</typeparam>
        /// <returns>The command promise.</returns>
        ICommandPromise Then<T>() where T : ICommand, new();

        /// <summary>
        /// Chains a new command provided by specific value.
        /// </summary>
        /// <param name="resultPromise">Result promise.</param>
        /// <param name="command">Command to chain.</param>
        /// <returns>The command promise.</returns>
        ICommandPromise Then(ICommandPromise resultPromise, ICommand command);

        /// <summary>
        /// Chains an enumerable of command promises, all of which must resolve.
        /// Returns a promise for a collection of the resolved results.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        ICommandPromise ThenAll(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);

        /// <summary>
        /// Chains an enumerable of command promises, all of which must resolve.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// It is rejected until all of the promises have been rejected.
        /// </summary>
        ICommandPromise ThenAny(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);

        /// <summary>
        /// Chains an enumerable of command promises, all of which must resolve.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        ICommandPromise ThenRace(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);

        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// Reports progress for the command.
        /// </summary>
        /// <param name="progress">The progress value.</param>
        void Progress(float progress);

        /// <summary>
        /// Handles exceptions for the command.
        /// </summary>
        /// <param name="e">The exception.</param>
        void Catch(Exception e);
    }
}
