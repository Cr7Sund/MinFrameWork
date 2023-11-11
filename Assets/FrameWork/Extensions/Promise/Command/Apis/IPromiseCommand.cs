using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommand<PromisedT> : IPromise<PromisedT>, IPoolable
    {
        /// <summary>
        /// chaining a new command provided by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IPromiseCommand<PromisedT> Then<T>() where T : IPromiseCommand<PromisedT>, new();

        IPromiseCommand<ConvertedT> Then<T, ConvertedT>() where T : IPromiseCommand<ConvertedT>, new();
        /// <summary>
        ///  chaining a new command provided by specific value
        /// </summary>
        /// <param name="resultPromise"></param>
        /// <returns></returns>
        IPromiseCommand<PromisedT> Then(IPromiseCommand<PromisedT> resultPromise);

        void Execute(object value);
        void Progress(float progress);
        void Catch(Exception e);
        /// <summary>
        /// the unit length of promise chain
        /// for progress calculation
        /// </summary>
        float SliceLength { get; set; }
        /// <summary>
        /// the sequence id of promise chain
        /// for progress calculation
        /// </summary>
        int SequenceID { get; set; }
    }
}