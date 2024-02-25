using Cr7Sund.FrameWork.Util;
using Cr7Sund.FrameWork.Test;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Cr7Sund.PackageTest.NodeTree
{
    [TestFixture]
    [TestOf(typeof(AsyncLoadable))]
    public class ResolveAsyncLoadableTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(new InternalLogger());
        }

        [Test]
        public void TestLoadAsync()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            sampleLoadable.LoadAsync();

            // Assert
            Assert.AreEqual(LoadState.Loaded, sampleLoadable.State);
            Assert.IsNotNull(sampleLoadable.LoadStatus);
        }

        [Test]
        public void TestLoadAndUnloadCompleted()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            sampleLoadable.LoadAsync();
            sampleLoadable.UnloadAsync();

            // Assert
            Assert.AreEqual(LoadState.Unloaded, sampleLoadable.State);
        }

        [Test]
        public void TestLoadRejected()
        {
            LogAssert.ignoreFailingMessages = true;

            // Arrange
            var sampleLoadable = new RejectableAsyncLoadable();

            // Act
            sampleLoadable.LoadAsync();

            // Assert
            Assert.AreEqual(LoadState.Fail, sampleLoadable.State);
        }

        [Test]
        public void TestLoadException()
        {
            // Arrange
            var sampleLoadable = new SampleAsyncLoadableWithException();

            // Act
            sampleLoadable.LoadAsync();

            // Assert
            Assert.AreEqual(LoadState.Fail, sampleLoadable.State);
        }

        [Test]
        public void TestUnloadAsync_Exception()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            TestDelegate handler = () => sampleLoadable.UnloadAsync();

            // Assert
            var ex = Assert.Throws<MyException>(handler);
            Assert.AreEqual(NodeTreeExceptionType.UNLOAD_VALID_STATE, ex.Type);
        }

    }

}
