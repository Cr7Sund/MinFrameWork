using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using Cr7Sund.FrameWork.Test;
// using Cr7Sund.FrameWork.Test;

namespace Cr7Sund.PackageTest.NodeTree
{
    [TestFixture]
    [TestOf(typeof(AsyncLoadable))]
    public class ResolveAsyncLoadableTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }

        [Test]
        public async Task TestLoadAsync()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            await sampleLoadable.LoadAsync();

            // Assert
            Assert.AreEqual(LoadState.Loaded, sampleLoadable.LoadState);
        }

        [Test]
        public async Task TestLoadAndUnloadCompleted()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            await sampleLoadable.LoadAsync();
            await sampleLoadable.UnloadAsync();

            // Assert
            Assert.AreEqual(LoadState.Unloaded, sampleLoadable.LoadState);
        }

        [Test]
        public async Task TestLoadRejected()
        {
            AssertHelper.IgnoreFailingMessages();

            // Arrange
            var sampleLoadable = new RejectableAsyncLoadable();

            // Act
            try
            {
                await sampleLoadable.LoadAsync();
            }
            catch
            {
            }

            // Assert
            Assert.AreEqual(LoadState.Fail, sampleLoadable.LoadState);
        }

        [Test]
        public async Task TestLoadException()
        {
            // Arrange
            var sampleLoadable = new SampleAsyncLoadableWithException();

            // Act
            MyException ex = null;
            try
            {
                await sampleLoadable.LoadAsync();
            }
            catch (MyException e)
            {
                ex = e;
            }

            // Assert
            Assert.NotNull(ex);
            Assert.AreEqual(LoadState.Fail, sampleLoadable.LoadState);
        }

        [Test]
        public async Task TestUnloadAsync_Exception()
        {
            // Arrange
            var sampleLoadable = new ResolveAsyncLoadable();

            // Act
            MyException ex = null;
            try
            {
                await sampleLoadable.UnloadAsync();
            }
            catch (MyException e)
            {
                ex = e;
            }
            // Assert
            Assert.AreEqual(NodeTreeExceptionType.UNLOAD_VALID_STATE, ex.Type);
        }

    }

}
