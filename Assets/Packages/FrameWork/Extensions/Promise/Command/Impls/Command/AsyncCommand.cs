using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
{
    public abstract class AsyncCommand<PromisedT> : Command<PromisedT>, IAsyncCommand<PromisedT>
    {

        public virtual IPromise<PromisedT> OnCatchAsync(Exception ex) { return null; }

        public abstract IPromise<PromisedT> OnExecuteAsync(PromisedT value);
        public sealed override PromisedT OnExecute(PromisedT value)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AsyncCommand<PromisedT, ConvertedT> : Command<PromisedT, ConvertedT>, IPromiseAsyncCommand<PromisedT, ConvertedT>
    {

        public abstract IPromise<ConvertedT> OnExecuteAsync(PromisedT value);
        public virtual IPromise<ConvertedT> OnCatchAsync(Exception ex) { return null; }
        public sealed override ConvertedT OnExecute(PromisedT value)
        {
            throw new NotImplementedException();
        }
    }

}
