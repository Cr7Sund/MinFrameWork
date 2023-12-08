using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public abstract class AsyncCommand : Command, IAsyncCommand
    {

        public virtual IPromise OnCatchAsync(Exception ex) { return null; }

        public abstract IPromise OnExecuteAsync();
        public sealed override void OnExecute()
        {
            throw new NotImplementedException();
        }
    }

}
