namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinding : IBinding
    {
        bool UsePooling { get; }
        ICommandPromise FirstPromise { get; }

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

        ICommandPromiseBinding AsPool();
    }
}
