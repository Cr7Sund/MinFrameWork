using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public abstract class AsyncCommand : Command, IAsyncCommand
    {
        public sealed override void OnExecute()
        {
            throw new NotImplementedException();
        }

        public virtual IPromise OnCatchAsync(Exception ex) { return null; }

        public abstract IPromise OnExecuteAsync();

    }

}