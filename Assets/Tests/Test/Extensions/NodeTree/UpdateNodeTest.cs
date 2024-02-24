using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.NodeTree
{
    public class UpdateNodeTest
    {
        private SampleUpdateNode _node;


        [SetUp]
        public void SetUp()
        {
            _node = new SampleUpdateNode();
            var context = new SampleRootContext();
            context.AddComponents(_node);
            _node.AssignContext(context);

            SampleUpdateNode.LateUpdateValue = 0;
            SampleUpdateNode.UpdateValue = 0;
            SampleChildUpdateNode.LateUpdateValue = 0;
            SampleChildUpdateNode.UpdateValue = 0;
        }

        [Test]
        public void AddChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleUpdateNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);

            // Assert
            Assert.AreEqual(1, _node.UpdateChildCount);
            Assert.AreEqual(1, _node.LateUpdateChildCount);
        }


        [Test]
        public void UpdateChild()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.Update(2);

            // Assert
            Assert.AreEqual(2, SampleUpdateNode.UpdateValue);
            Assert.AreEqual(4, SampleChildUpdateNode.UpdateValue);
        }

        [Test]
        public void LateUpdateChild()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.LateUpdate(2);

            // Assert
            Assert.AreEqual(2, SampleUpdateNode.LateUpdateValue);
            Assert.AreEqual(4, SampleChildUpdateNode.LateUpdateValue);
        }

        [Test]
        public void Update_NotStart()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Update(2);

            // Assert
            Assert.AreEqual(0, SampleUpdateNode.UpdateValue);
            Assert.AreEqual(0, SampleChildUpdateNode.UpdateValue);
        }

        [Test]
        public void LateUpdate_NotStart()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.LateUpdate(2);

            // Assert
            Assert.AreEqual(0, SampleUpdateNode.LateUpdateValue);
            Assert.AreEqual(0, SampleChildUpdateNode.LateUpdateValue);
        }
    }
}