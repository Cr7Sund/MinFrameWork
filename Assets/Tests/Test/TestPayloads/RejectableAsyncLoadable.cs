﻿using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
namespace Cr7Sund.FrameWork.Test
{
    public class RejectableAsyncLoadable : AsyncLoadable
    {
        public const string exMsg = "throw exception";

        protected override IPromise OnLoadAsync()
        {
            return Promise.Rejected(new Exception(exMsg));
        }

        protected override IPromise OnUnloadAsync()
        {
            return Promise.Rejected(new Exception("qw"));
        }

        protected override void OnCatch(Exception e)
        {
            Assert.AreEqual(exMsg, e.Message);
        }
    }
}
