using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
namespace Cr7Sund.FrameWork.Test
{
    [TestFixture]
    [TestOf(typeof(AsyncLoadable))]
    public class ResolveAsyncLoadableTests
    {

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
        public void TestLoadRejeced()
        {
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
           TestDelegate handler = ()=>  sampleLoadable.UnloadAsync();

            // Assert
            var ex = Assert.Throws<MyException>(handler);
            Assert.AreEqual(NodeTreeExceptionType.UNLOAD_VALID_STATE, ex.Type);
        }

    }

}
