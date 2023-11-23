using Cr7Sund.Framework.Impl;
using NUnit.Framework;
namespace Cr7Sund.Framework.Tests
{
    public class TestContextRemoval
    {
        private TestCrossContextSubclass Child;
        private CrossContext Parent;
        private object view;

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
