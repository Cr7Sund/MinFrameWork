namespace Cr7Sund.PackageTest.Api
{
    /// <summary>
    /// Represents a binding for command promises, extending IBinding.
    /// </summary>
    public interface ICommandPromiseBinding<PromisedT> : IBinding
    {
        /// <summary>
        /// Indicates the promise binding status.
        /// </summary>
        CommandBindingStatus BindingStatus { get; }

        /// <summary>
        /// Resets the promise status to start a new promise.
        /// </summary>
        void RestartPromise();

        /// <summary>
        /// Releases the instance and releases the promise to the pool to start a new promise.
        /// </summary>
        void RunPromise(PromisedT value);

        /// <summary>
        /// Declares that the binding is a one-off. As soon as it's satisfied, it will be unmapped.
        /// The promise command instantiated by the pool.
        /// </summary>
        ICommandPromiseBinding<PromisedT> AsOnce();

        /// <summary>
        /// Chains a new command provided by type.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        ICommandPromiseBinding<PromisedT> Then<T>() where T : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains a new command of a new type provided by specific type.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <typeparam name="ConvertedT">Type to convert to.</typeparam>
        ICommandPromiseBinding<PromisedT> Then<T, ConvertedT>() where T : class, ICommand<ConvertedT>, new();

        /// <summary>
        /// Chains a new command of a new type provided by specific value.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <typeparam name="ConvertedT">Type to convert to.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenConvert<T, ConvertedT>() where T : class, ICommand<PromisedT, ConvertedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when any of the commands have resolved.
        /// </summary>
        /// <param name="commands">Commands to chain.</param>
        ICommandPromiseBinding<PromisedT> ThenAny(params ICommand<PromisedT>[] commands);

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when any of the commands have resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenAny<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when any of the commands have resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        /// <typeparam name="T3">Type of the third command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenAny<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <param name="commands">Commands to chain.</param>
        ICommandPromiseBinding<PromisedT> ThenFirst(params ICommand<PromisedT>[] commands);

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenFirst<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        /// <typeparam name="T3">Type of the third command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenFirst<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <param name="commands">Commands to chain.</param>
        ICommandPromiseBinding<PromisedT> ThenRace(params ICommand<PromisedT>[] commands);

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenRace<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        /// <summary>
        /// Chains multiple commands and returns a promise that resolves when the first of the commands has resolved.
        /// </summary>
        /// <typeparam name="T1">Type of the first command.</typeparam>
        /// <typeparam name="T2">Type of the second command.</typeparam>
        /// <typeparam name="T3">Type of the third command.</typeparam>
        ICommandPromiseBinding<PromisedT> ThenRace<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();
    }

    /// <summary>
    /// Represents the status of a command binding.
    /// </summary>
    public enum CommandBindingStatus
    {
        Default,
        Released,
        Running,
    }
}
