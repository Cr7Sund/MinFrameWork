using System;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.FrameWork.Test
{
    public class RejectableAsyncLoadable : AsyncLoadable
    {
        public const string exMsg = "throw exception";

        protected override PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation)
        {
            throw new Exception(exMsg);
        }

        protected override PromiseTask OnUnloadAsync(UnsafeCancellationToken cancellation)
        {
            throw new Exception("qw");
        }

        protected override void OnCatch(Exception e)
        {
            // AssertUtil.AreEqual(exMsg, e.Message);
        }


    }
}
