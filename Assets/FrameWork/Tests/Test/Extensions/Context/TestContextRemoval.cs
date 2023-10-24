using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    public class TestContextRemoval
    {
        object view;
        CrossContext Parent;
        TestCrossContextSubclass Child;

        [SetUp]
        public void SetUp()
        {
            Context.FirstContext = null;
            view = new object();
            Parent = new CrossContext(view, true);
            Child = new TestCrossContextSubclass(view, true);
        }

        [Test]
        public void TestRemoval()
        {
            Parent.AddContext(Child);

            TestDelegate testDelegate = delegate
            {
                Parent.RemoveContext(Child);
            };

            Assert.Throws<TestPassedException>(testDelegate);
        }
    }
}