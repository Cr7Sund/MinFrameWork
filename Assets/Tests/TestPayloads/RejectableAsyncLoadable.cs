using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
using System.Threading;
namespace Cr7Sund.FrameWork.Test
{
    public class RejectableAsyncLoadable : AsyncLoadable
    {
        public const string exMsg = "throw exception";

        protected override PromiseTask OnLoadAsync()
        {
            throw new Exception(exMsg);
        }

        protected override PromiseTask OnUnloadAsync()
        {
            throw new Exception("qw");
        }

        protected override void OnCatch(Exception e)
        {
            Assert.AreEqual(exMsg, e.Message);
        }


    }
}
