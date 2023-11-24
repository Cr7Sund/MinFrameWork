namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinding : IBinding
    {
        // the fist promise to start
        ICommandPromise FirstPromise { get; }
        /// Indicate the promise binding status
        CommandBindingStatus BindingStatus { get; }
        /// Declares that the promise command instantiated by pool.
        ICommandPromiseBinding AsPool();
        /// Declares that the Binding is a one-off. As soon as it's satisfied, it will be unmapped.
        ICommandPromiseBinding AsOnce();
        /// reset promise status 
        /// to start new promise 
        void RestartPromise();
        /// release instance and release promise to pool 
        /// to start new promise 
        void RunPromise();
        ICommandPromiseBinding Then<T>() where T : class, ICommand, new();
        ICommandPromiseBinding ThenAny(params ICommand[] commands);
        ICommandPromiseBinding ThenAny<T1, T2>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new();
        ICommandPromiseBinding ThenAny<T1, T2, T3>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new();

        ICommandPromiseBinding ThenRace(params ICommand[] commands);
        ICommandPromiseBinding ThenRace<T1, T2>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new();
        ICommandPromiseBinding ThenRace<T1, T2, T3>()
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new();

    }
}
