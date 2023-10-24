using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Tests
{
    public class TestCrossContextSubclass : CrossContext
    {

        public TestCrossContextSubclass(object view, bool autoStartup) : base(view, autoStartup)
        { }

        public override void OnRemove()
        {
            base.OnRemove();

            throw new TestPassedException("Test Passed");
        }
    }

    class TestPassedException : Exception
    {
        public TestPassedException(string str) : base(str) { }
    }
}