using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.FrameWork.Test
{
    public class ResolveAsyncLoadable : AsyncLoadable
    {
        protected override IPromise OnLoadAsync()
        {

            return Promise.Resolved();
        }

        protected override IPromise OnUnloadAsync()
        {

            return Promise.Resolved();
        }
    }
}
