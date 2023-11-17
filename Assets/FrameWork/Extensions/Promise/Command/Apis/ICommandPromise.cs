using System;
using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{

    public interface ICommandPromise<PromisedT> : IPromise<PromisedT>, ISequence, IPoolable
    {
        /// <summary>
        /// chaining a new command provided by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandPromise<PromisedT> Then<T>() where T : IPromiseCommand<PromisedT>, new();

        // ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommandPromise<ConvertedT>, new();
        /// <summary>
        ///  chaining a new command provided by specific value
        /// </summary>
        /// <param name="resultPromise"></param>
        /// <returns></returns>
        ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, IPromiseCommand<PromisedT> command);

        ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : IPromiseCommand<PromisedT, ConvertedT>, new();
        ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise, IPromiseCommand<PromisedT, ConvertedT> command);

        ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands);

        void Execute(PromisedT value);
        void Progress(float progress);
        void Catch(Exception e);
    }

}