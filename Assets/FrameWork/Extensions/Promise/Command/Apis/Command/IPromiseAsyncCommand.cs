using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseAsyncCommand : IBaseCommand
    {
        IPromise OnExecuteAsync();
        IPromise OnCatchAsync(Exception ex);
    }


    public interface IPromiseAsyncCommand<PromisedT> : IBaseCommand
    {
        IPromise<PromisedT> OnExecuteAsync(PromisedT value);
        IPromise<PromisedT> OnCatchAsync(Exception ex);
    }


    public interface IPromiseAsyncCommand<PromisedT, ConvertedT> : IBaseCommand
    {
        IPromise<ConvertedT> OnExecuteAsync(PromisedT value);
        IPromise<ConvertedT> OnCatchAsync(Exception ex);
    }
}