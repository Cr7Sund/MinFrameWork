using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
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
