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
        ICommandPromise<PromisedT> Then<T>() where T : ICommand<PromisedT>, new();

        // ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommandPromise<ConvertedT>, new();
        /// <summary>
        ///  chaining a new command provided by specific value
        /// </summary>
        /// <param name="resultPromise"></param>
        /// <returns></returns>
        ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, ICommand<PromisedT> command);

        ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommand<PromisedT, ConvertedT>, new();
        ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise, ICommand<PromisedT, ConvertedT> command);

        ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);
        ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<ICommand<PromisedT>> commands);

        void Execute(PromisedT value);
        void Progress(float progress);
        void Catch(Exception e);
    }

}