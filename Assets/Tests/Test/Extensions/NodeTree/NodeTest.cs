using System.Reflection;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
namespace Cr7Sund.PackageTest.NodeTree
{
    [TestFixture]
    [TestOf(typeof(Node))]
    public class NodeTest
    {
        private SampleNode _node;
        [SetUp]
        public void SetUp()
        {
            _node = new SampleNode();
            var context = new SampleRootContext();
            context.AddComponents(_node);
            _node.AssignContext(context);
        }


        [Test]
        public void AddChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);

            // Assert
            Assert.AreEqual(2, _node.ChildCount);
        }

        [Test]
        public void AddChild_DefaultState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);


            // Assert
            Assert.IsFalse(_node.IsInit);
            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(_node.IsStarted);

            Assert.IsTrue(child1.IsInit);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child1.IsStarted);

            Assert.IsTrue(child2.IsInit);
            Assert.IsFalse(child2.IsActive);
            Assert.IsFalse(child2.IsStarted);
        }



        [Test]
        public void AddChild_Init()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();

            // Assert
            Assert.IsTrue(_node.IsInit);
            Assert.IsTrue(child1.IsInit);
            Assert.IsTrue(child2.IsInit);
        }

        [Test]
        public void AddChild_Start()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();

            // Assert
            Assert.IsTrue(_node.IsStarted);
            Assert.IsTrue(child1.IsStarted);
            Assert.IsTrue(child2.IsStarted);
        }

        [Test]
        public void AddChild_Enable()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();

            // Assert
            Assert.IsTrue(_node.IsActive);
            Assert.IsTrue(child1.IsActive);
            Assert.IsTrue(child2.IsActive);
        }

        [Test]
        public void AddChild_Enable_Not_Start()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Enable();

            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public void ActiveChild_BeforeAdd()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);

            // Assert
            Assert.IsTrue(_node.IsInit);
            Assert.IsTrue(_node.IsActive);
            Assert.IsTrue(_node.IsStarted);

            Assert.IsTrue(child1.IsInit);
            Assert.IsTrue(child1.IsActive);
            Assert.IsTrue(child1.IsStarted);

            Assert.IsTrue(child2.IsInit);
            Assert.IsTrue(child2.IsActive);
            Assert.IsTrue(child2.IsStarted);
        }

        [Test]
        public void RemoveChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.RemoveChildAsync(child2);

            Assert.AreEqual(1, _node.ChildCount);
        }


        [Test]
        public void RemoveAllChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.RemoveChildAsync(child2);
            _node.RemoveChildAsync(child1);

            Assert.AreEqual(0, _node.ChildCount);
        }


        [Test]
        public void RemoveChild_State()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.RemoveChildAsync(child2);
            _node.RemoveChildAsync(child1);

            // Assert
   
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public void UnloadChild_State()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.UnloadChildAsync(child2);
            _node.UnloadChildAsync(child1);

            // Assert
            Assert.IsFalse(child1.IsStarted);
            Assert.IsFalse(child2.IsStarted);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public void DisposeAll()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.AddChildAsync(child1);
            _node.AddChildAsync(child2);
            _node.Init();
            _node.Start();
            _node.Enable();
            _node.Dispose();

            // Assert
            Assert.IsFalse(_node.IsInit);
            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(_node.IsStarted);

            Assert.IsFalse(child1.IsInit);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child1.IsStarted);

            Assert.IsFalse(child2.IsInit);
            Assert.IsFalse(child2.IsActive);
            Assert.IsFalse(child2.IsStarted);
        }
        [Test]
        public void EmptyCrossContext()
        {

            // Arrange
            var emptyCrossContextNode = new SampleNode();

            var child1 = new SampleNode();

            // Act
            TestDelegate handler = () => emptyCrossContextNode.AddChildAsync(child1);

            var ex = Assert.Throws<MyException>(handler);
            Assert.AreEqual(ex.Type, NodeTreeExceptionType.EMPTY_CROSS_CONTEXT);
        }
    }
}
