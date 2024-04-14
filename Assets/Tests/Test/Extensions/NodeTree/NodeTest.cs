using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
namespace Cr7Sund.PackageTest.NodeTree
{
    [TestFixture]
    public class NodeTest
    {
        public static readonly CancellationTokenSource UnitCancellation = new CancellationTokenSource();

        private SampleNode _node;
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
            _node = new SampleNode();
            var context = new SampleRootContext();
            context.AddComponents(_node);
            _node.AssignContext(context);

        }


        [Test]
        public async Task AddChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);

            // Assert
            Assert.AreEqual(2, _node.ChildCount);
        }

        [Test]
        public async Task AddChild_DefaultState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);

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
        public async Task AddChild_Init()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();

            // Assert
            Assert.IsTrue(_node.IsInit);
            Assert.IsTrue(child1.IsInit);
            Assert.IsTrue(child2.IsInit);
        }

        [Test]
        public async Task AddChild_Start()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start();

            // Assert
            Assert.IsTrue(_node.IsStarted);
            Assert.IsTrue(child1.IsStarted);
            Assert.IsTrue(child2.IsStarted);
        }

        [Test]
        public async Task AddChild_Enable()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start();
            await _node.Enable();

            // Assert
            Assert.IsTrue(_node.IsActive);
            Assert.IsTrue(child1.IsActive);
            Assert.IsTrue(child2.IsActive);
        }

        [Test]
        public async Task AddChild_Enable_Not_Start()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Enable();

            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public async Task ActiveChild_BeforeAdd()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.Init();
            await _node.Start();
            await _node.Enable();
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);

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
        public async Task DisableChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.RemoveChildAsync(child2);

            Assert.AreEqual(1, _node.ChildCount);
        }


        [Test]
        public async Task RemoveAllChild()
        {
            _node.Init();
            await _node.Start();
            await _node.Enable();

            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.RemoveChildAsync(child2);
            await _node.RemoveChildAsync(child1);

            Assert.AreEqual(0, _node.ChildCount);
        }


        [Test]
        public async Task RemoveChild_State()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start();
            await _node.Enable();
            await _node.RemoveChildAsync(child2);
            await _node.RemoveChildAsync(child1);

            // Assert

            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public async Task UnloadAllChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start();
            await _node.Enable();
            await _node.UnloadChildAsync(child2);
            await _node.UnloadChildAsync(child1);

            // Assert
            Assert.IsFalse(child1.IsStarted);
            Assert.IsFalse(child2.IsStarted);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public async Task UnloadAllNode()
        {
            _node.Init();
            await _node.LoadAsync();

            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.Start();
            await _node.Enable();
            await _node.UnloadChildAsync(child1);
            await _node.UnloadChildAsync(child2);

            await DestroyRootNode(_node);

            // Assert
            Assert.IsFalse(_node.IsInit);
            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(_node.IsStarted);
            Assert.AreEqual(_node.LoadState, LoadState.Unloaded);

            Assert.IsFalse(child1.IsInit);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child1.IsStarted);

            Assert.IsFalse(child2.IsInit);
            Assert.IsFalse(child2.IsActive);
            Assert.IsFalse(child2.IsStarted);

            _node.Dispose();
            Assert.AreEqual(_node.LoadState, LoadState.Default);
        }
        [Test]
        public async Task EmptyCrossContext()
        {
            MyException ex = null;
            // Arrange
            var emptyCrossContextNode = new SampleNode();
            var child1 = new SampleNode();

            // Act
            try
            {
                await emptyCrossContextNode.AddChildAsync(child1);
            }
            catch (MyException e)
            {
                ex = e;
            }

            Assert.AreEqual(ex.Type, NodeTreeExceptionType.EMPTY_CROSS_CONTEXT);
        }

        private async PromiseTask DestroyRootNode(Node root)
        {
            if (!root.IsInit) return;
            if (root.IsActive)
            {
                await root.SetActive(false);
            }
            if (root.IsStarted)
            {
                await root.Stop();
            }

            if (root.LoadState == LoadState.Loading || root.LoadState == LoadState.Loaded)
            {
                await root.UnloadAsync();
            }

            root.Destroy();
        }
    }
}
