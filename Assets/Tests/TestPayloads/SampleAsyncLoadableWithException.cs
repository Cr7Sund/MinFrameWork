﻿using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
namespace Cr7Sund.FrameWork.Test
{
    public class SampleAsyncLoadableWithException : AsyncLoadable
    {
        public const string exMsg = "throw exception";
        protected override IPromise OnLoadAsync()
        {
            // Simulate an exception during the loading process
            throw new MyException(exMsg);
        }

        protected override IPromise OnUnloadAsync()
        {
            // Implement unloading logic
            return Promise.Resolved();
        }
        protected override void OnCatch(Exception e)
        {
            Assert.AreEqual(exMsg, e.Message);
        }
    }
}
