using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
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
