namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinding<PromisedT> : IBinding
    {
        /// Indicate the promise binding status
        CommandBindingStatus BindingStatus { get; }

        /// reset promise status 
        /// to start new promise 
        void RestartPromise();
        /// release instance and release promise to pool 
        /// to start new promise 
        void RunPromise(PromisedT value);
        /// Declares that the Binding is a one-off. As soon as it's satisfied, it will be unmapped.
        /// he promise command instantiated by pool.
        ICommandPromiseBinding<PromisedT> AsOnce();


        ICommandPromiseBinding<PromisedT> Then<T>() where T : class, ICommand<PromisedT>, new();
        ICommandPromiseBinding<PromisedT> Then<T, ConvertedT>() where T : class, ICommand<ConvertedT>, new();
        ICommandPromiseBinding<PromisedT> ThenConvert<T, ConvertedT>() where T : class, ICommand<PromisedT, ConvertedT>, new();


        ICommandPromiseBinding<PromisedT> ThenAny(params ICommand<PromisedT>[] commands);
        ICommandPromiseBinding<PromisedT> ThenAny<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        ICommandPromiseBinding<PromisedT> ThenAny<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();

        ICommandPromiseBinding<PromisedT> ThenFirst(params ICommand<PromisedT>[] commands);
        ICommandPromiseBinding<PromisedT> ThenFirst<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        ICommandPromiseBinding<PromisedT> ThenFirst<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();


        ICommandPromiseBinding<PromisedT> ThenRace(params ICommand<PromisedT>[] commands);
        ICommandPromiseBinding<PromisedT> ThenRace<T1, T2>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new();

        ICommandPromiseBinding<PromisedT> ThenRace<T1, T2, T3>()
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new();
    }

    public enum CommandBindingStatus
    {
        Default,
        Released,
        Running,
    }
}
