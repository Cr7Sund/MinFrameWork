using System;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Api
{

    public interface ICommandPromise<PromisedT> : IPromise<PromisedT>, ISequence, IPoolable,IResetable
    {
        Action<PromisedT> ExecuteHandler { get; }
        Action<float> SequenceProgressHandler { get; }
        Action<float> CommandProgressHandler { get; }


        /// <summary>
        ///     chaining a new command provided by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandPromise<PromisedT> Then<T>() where T : ICommand<PromisedT>, new();

        /// <summary>
        ///     chaining a new command provided by specific value
        /// </summary>
        /// <param name="resultPromise"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, ICommand<PromisedT> command);

        /// <summary>
        ///     chaining a new command of new type provided by specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ConvertedT"></typeparam>
        /// <returns></returns>
        ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommand<PromisedT, ConvertedT>, new();
   
        /// <summary>
        ///     chaining a new command of new type provided by specific value
        /// </summary>
        /// <param name="resultPromise"></param>
        /// <param name="command"></param>
        /// <typeparam name="ConvertedT"></typeparam>
        /// <returns></returns>
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
