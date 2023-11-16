using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinding<PromisedT> : IBinding
    {
        bool UsePooling { get; set; }

        IPromiseCommandBinding<PromisedT> Then<T>() where T : class, IPromiseCommand<PromisedT>, new();
        IPromiseCommandBinding<PromisedT> Then<T, ConvertedT>() where T : class, IPromiseCommand<ConvertedT>, new();
        IPromiseCommandBinding<PromisedT> ThenConvert<T, ConvertedT>() where T : class, IPromiseCommand<PromisedT, ConvertedT>, new();


        IPromiseCommandBinding<PromisedT> ThenAny(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding<PromisedT> ThenAny<T1, T2>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding<PromisedT> ThenAny<T1, T2, T3>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding<PromisedT> ThenFirst(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding<PromisedT> ThenFirst<T1, T2>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding<PromisedT> ThenFirst<T1, T2, T3>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();


        IPromiseCommandBinding<PromisedT> ThenRace(params IPromiseCommand<PromisedT>[] commands);
        IPromiseCommandBinding<PromisedT> ThenRace<T1, T2>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new();

        IPromiseCommandBinding<PromisedT> ThenRace<T1, T2, T3>()
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new();
    }
}