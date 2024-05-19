#pragma warning disable CS4014

using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using Cr7Sund.Package.Impl;

namespace Cr7Sund.PackageTest.NodeTree
{
    [TestFixture]
    public class NodeTest
    {

        private SampleNode _node;

        [SetUp]
        public void NodeTest_SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
            _node = new SampleNode();
            var context = new SampleRootContext();
            context.AddComponents(_node);
            _node.AssignContext(context);
            SampleCancelNode.LoadPromise = Promise.Resolved();
        }

        [Test]
        public async Task NodeTest_AddChild_ShouldAddChildCount()
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
        public async Task NodeTest_AddChild_ShouldNotChangeState()
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
        public async Task NodeTest_AddChild_Init_ShouldChangeState()
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
        public async Task NodeTest_AddChild_Start_ShouldChangeState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);

            // Assert
            Assert.IsTrue(_node.IsStarted);
            Assert.IsTrue(child1.IsStarted);
            Assert.IsTrue(child2.IsStarted);
        }

        [Test]
        public async Task NodeTest_AddChild_Enable_ShouldChangeState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);
            await _node.Enable();

            // Assert
            Assert.IsTrue(_node.IsActive);
            Assert.IsTrue(child1.IsActive);
            Assert.IsTrue(child2.IsActive);
        }

        [Test]
        public async Task NodeTest_AddChild_Enable_NotStart_ShouldNotChangeState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Enable();

            // Assert
            Assert.IsFalse(_node.IsActive);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public async Task NodeTest_ActiveChild_BeforeAdd_ShouldChangeState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            _node.Init();
            await _node.Start(default);
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
        public async Task NodeTest_DisableChild_ShouldReduceChildCount()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.RemoveChildAsync(child2);

            // Assert
            Assert.AreEqual(1, _node.ChildCount);
        }

        [Test]
        public async Task NodeTest_RemoveAllChild_ShouldRemoveAllChildren()
        {
            // Arrange
            _node.Init();
            await _node.Start(default);
            await _node.Enable();

            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.RemoveChildAsync(child2);
            await _node.RemoveChildAsync(child1);

            // Assert
            Assert.AreEqual(0, _node.ChildCount);
        }

        [Test]
        public async Task NodeTest_RemoveChild_State_ShouldChangeState()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);
            await _node.Enable();
            await _node.RemoveChildAsync(child2);
            await _node.RemoveChildAsync(child1);

            // Assert
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child2.IsActive);
        }

        [Test]
        public async Task NodeTest_UnloadAllChild_ShouldUnloadAllChildren()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);
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
        public async Task NodeTest_UnloadAllNode_ShouldUnloadAllNodes()
        {
            _node.Init();
            await _node.LoadAsync(UnsafeCancellationToken.None);

            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            await _node.Start(default);
            await _node.Enable();
            await _node.UnloadChildAsync(child1);
            await _node.UnloadChildAsync(child2);

            await DestroyRootNode(_node);

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

            _node.Dispose();
            Assert.AreEqual(_node.LoadState, LoadState.Unloaded);
        }

        [Test]
        public async Task NodeTest_EmptyCrossContext_ShouldThrowException()
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

            // Assert
            Assert.AreEqual(ex.Type, NodeTreeExceptionType.EMPTY_CROSS_CONTEXT);
        }

        [Test]
        public async Task NodeTest_PreloadNode_ShouldNotChangeState()
        {
            var child1 = new SampleNode();

            await _node.Start(default);
            await _node.Enable();
            await _node.PreLoadChild(child1);

            Assert.IsFalse(child1.IsInit);
            Assert.IsFalse(child1.IsActive);
            Assert.IsFalse(child1.IsStarted);
        }

        [Test]
        public async Task NodeTest_PreloadNode_ShouldChangeState()
        {
            var child1 = new SampleNode();

            await _node.Start(default);
            await _node.Enable();
            await _node.PreLoadChild(child1);
            await _node.AddChildAsync(child1);

            Assert.IsTrue(child1.IsInit);
            Assert.IsTrue(child1.IsActive);
            Assert.IsTrue(child1.IsStarted);
        }

        [Test]
        public async Task NodeTest_PreloadNode_NotAddChildCount()
        {
            var child1 = new SampleNode();

            await _node.Start(default);
            await _node.Enable();
            await _node.PreLoadChild(child1);

            Assert.AreEqual(0, _node.ChildCount);
        }

        [Test]
        public async Task NodeTest_PreloadNode_AddChildCount()
        {
            var child1 = new SampleNode();

            await _node.Start(default);
            await _node.Enable();
            await _node.PreLoadChild(child1);
            await _node.AddChildAsync(child1);

            Assert.AreEqual(1, _node.ChildCount);
        }

        [Test]
        public async Task NodeTest_CancelNode_ShouldNotCancelLoaded()
        {
            var child1 = new SampleNode();

            await _node.Start(default);
            await _node.Enable();
            await _node.AddChildAsync(child1);
            child1.CancelLoad();

            Assert.AreEqual(1, _node.ChildCount);
            Assert.IsTrue(child1.IsInit);
            Assert.IsTrue(child1.IsStarted);
        }


        [Test]
        public async Task NodeTest_CancelNode_ChangeToFailState()
        {
            SampleCancelNode.LoadPromise = new Promise();
            var child1 = new SampleCancelNode();

            await _node.Start(default);
            await _node.Enable();
            _node.AddChildAsync(child1);
            child1.CancelLoad();

            Assert.AreEqual(LoadState.Unloaded, child1.LoadState);
            Assert.AreEqual(0, _node.ChildCount);
        }

        [Test]
        public async Task NodeTest_CancelNode_ReLoad()
        {
            SampleCancelNode.LoadPromise = new Promise();
            var child1 = new SampleCancelNode();

            await _node.Start(default);
            await _node.Enable();
            _node.AddChildAsync(child1);
            child1.CancelLoad();

            Assert.AreEqual(0, _node.ChildCount);
            Assert.IsFalse(child1.IsInit);
            Assert.IsFalse(child1.IsStarted);
        }

        [Test]
        public async Task NodeTest_ReAddCancelNode_ShouldChangeState()
        {
            SampleCancelNode.LoadPromise = new Promise();
            var child1 = new SampleCancelNode();

            await _node.Start(default);
            await _node.Enable();

            _node.AddChildAsync(child1);
            child1.CancelLoad();
            child1.AssignContext(new SampleContext());
            SampleCancelNode.LoadPromise = new Promise();
            SampleCancelNode.LoadPromise.Resolve();
            await _node.AddChildAsync(child1);

            Assert.AreEqual(LoadState.Loaded, child1.LoadState);
            Assert.AreEqual(NodeState.Ready, child1.NodeState);
            Assert.AreEqual(1, _node.ChildCount);
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
                await root.UnloadAsync(UnsafeCancellationToken.None);
            }

            root.Destroy(default);
        }
    }
}
