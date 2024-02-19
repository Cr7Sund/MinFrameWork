using System;

namespace Cr7Sund.PackageTest.Api
{
    /// <summary>
    /// Represents a binding for command promises, extending the generic IBinding interface with additional command-specific functionalities.
    /// </summary>
    public interface ICommandPromiseBinding : IBinding
    {
        /// <summary>
        /// Gets the promise binding status.
        /// </summary>
        CommandBindingStatus BindingStatus { get; }

        /// <summary>
        /// Declares that the binding is a one-off. As soon as it's satisfied, it will be unmapped.
        /// The promise command instantiated by the pool.
        /// </summary>
        ICommandPromiseBinding AsOnce();

        /// <summary>
        /// Resets the promise status to start a new promise.
        /// </summary>
        void RestartPromise();

        /// <summary>
        /// Releases the instance and releases the promise to the pool to start a new promise.
        /// </summary>
        void RunPromise();

        /// <summary>
        /// Chains a new command provided by type.
        /// </summary>
        /// <typeparam name="T">The type of the command.</typeparam>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding Then<T>() where T : class, ICommand, new();

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <param name="commands">The array of commands to chain.</param>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenAny(params ICommand[] commands);

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <typeparam name="T1">The type of the first command.</typeparam>
        /// <typeparam name="T2">The type of the second command.</typeparam>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenAny<T1, T2>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new();

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <typeparam name="T1">The type of the first command.</typeparam>
        /// <typeparam name="T2">The type of the second command.</typeparam>
        /// <typeparam name="T3">The type of the third command.</typeparam>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenAny<T1, T2, T3>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new();

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <param name="commands">The array of commands to chain.</param>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenRace(params ICommand[] commands);

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <typeparam name="T1">The type of the first command.</typeparam>
        /// <typeparam name="T2">The type of the second command.</typeparam>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenRace<T1, T2>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new();

        /// <summary>
        /// Chains multiple commands and returns a command promise binding that resolves when any of the commands resolves.
        /// </summary>
        /// <typeparam name="T1">The type of the first command.</typeparam>
        /// <typeparam name="T2">The type of the second command.</typeparam>
        /// <typeparam name="T3">The type of the third command.</typeparam>
        /// <returns>The command promise binding.</returns>
        ICommandPromiseBinding ThenRace<T1, T2, T3>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new();
    }
}
