using Cr7Sund.PackageTest.IOC;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Cr7Sund.PackageTest.NodeTree
{
    public class UpdateNodeTest
    {
        public static readonly CancellationTokenSource UnitCancellation = new CancellationTokenSource();

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
        public async Task AddChild()
        {
            // Arrange
            var child1 = new SampleNode();
            var child2 = new SampleUpdateNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);

            // Assert
            Assert.AreEqual(1, _node.UpdateChildCount);
            Assert.AreEqual(1, _node.LateUpdateChildCount);
        }


        [Test]
        public async Task UpdateChild()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);
            await _node.Enable();
            _node.Update(2);

            // Assert
            Assert.AreEqual(2, SampleUpdateNode.UpdateValue);
            Assert.AreEqual(4, SampleChildUpdateNode.UpdateValue);
        }

        [Test]
        public async Task LateUpdateChild()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Init();
            await _node.Start(default);
            await _node.Enable();
            _node.LateUpdate(2);

            // Assert
            Assert.AreEqual(2, SampleUpdateNode.LateUpdateValue);
            Assert.AreEqual(4, SampleChildUpdateNode.LateUpdateValue);
        }

        [Test]
        public async Task Update_NotStart()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.Update(2);

            // Assert
            Assert.AreEqual(0, SampleUpdateNode.UpdateValue);
            Assert.AreEqual(0, SampleChildUpdateNode.UpdateValue);
        }

        [Test]
        public async Task LateUpdate_NotStart()
        {
            // Arrange
            var child1 = new SampleChildUpdateNode();
            var child2 = new SampleChildUpdateNode();

            // Act
            await _node.AddChildAsync(child1);
            await _node.AddChildAsync(child2);
            _node.LateUpdate(2);

            // Assert
            Assert.AreEqual(0, SampleUpdateNode.LateUpdateValue);
            Assert.AreEqual(0, SampleChildUpdateNode.LateUpdateValue);
        }
    }
}