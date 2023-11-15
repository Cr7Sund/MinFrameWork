using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseAsyncCommand : PromiseCommand, IPromiseAsyncCommand
    {
        public sealed override void OnExecute()
        {
            throw new NotImplementedException();
        }

        public virtual IPromise OnCatchAsync(Exception ex) { return null; }

        public abstract IPromise OnExecuteAsync();

    }

}