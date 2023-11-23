using Cr7Sund.Framework.Impl;
using System;
namespace Cr7Sund.Framework.Tests
{
    public class TestCrossContextSubclass : CrossContext
    {

        public TestCrossContextSubclass(object view, bool autoStartup) : base(view, autoStartup)
        {
        }

        public override void OnRemove()
        {
            base.OnRemove();

            throw new TestPassedException("Test Passed");
        }
    }

    internal class TestPassedException : Exception
    {
        public TestPassedException(string str) : base(str) { }
    }
}
