using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinding : IBinding
    {
        bool UsePooling { get; set; }

        IPromiseCommandBinding Then<T>() where T : class, IPromiseCommand, new();
        IPromiseCommandBinding Then<T, PromisedT>() where T : class, IPromiseCommand<PromisedT>, new();
        IPromiseCommandBinding Then<T, PromisedT, ConvertedT>() where T : class, IPromiseCommand<PromisedT, ConvertedT>, new();
        IPromiseCommandBinding ThenAll<PromisedT>(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding ThenAll<T1, T2, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenAll<T1, T2, T3, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenAny<PromisedT>(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding ThenAny<T1, T2, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenAny<T1, T2, T3, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenFirst<PromisedT>(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding ThenFirst<T1, T2, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenFirst<T1, T2, T3, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenRace<PromisedT>(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding ThenRace<T1, T2, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding ThenRace<T1, T2, T3, PromisedT>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();
    }
}