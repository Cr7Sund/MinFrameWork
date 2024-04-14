using Cr7Sund.Server.Impl;
using NUnit.Framework;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.Tests;
using System.Threading.Tasks;

namespace Cr7Sund.ServerTest.Scene
{
    public class TestSceneModule
    {
        private ISceneModule _sceneModule;

        [SetUp]
        public async Task SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());

            _sceneModule = new SceneModule();
            var gameLogic = new SampleGameLogic();
            gameLogic.Init();
            await gameLogic.Run();

            gameLogic.GetContextInjector().Inject(_sceneModule);

            SampleSceneOneController.Init();
            SampleSceneTwoController.Init();
        }

        [Test]
        public async Task AddScene()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            var node = ((LoadModule)_sceneModule).TestGetViewByKey<SceneNode>(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(NodeState.Ready, node.NodeState);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task SwitchToFistScene()
        {
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne);
            var node = ((LoadModule)_sceneModule).TestGetViewByKey<SceneNode>(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(NodeState.Ready, node.NodeState);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task SwitchToNewScene()
        {

            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task AddScenesAsync()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task RemoveScene()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            var node = ((LoadModule)_sceneModule).TestGetViewByKey<SceneNode>(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(NodeState.Removed, node.NodeState);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task RemoveScenes()
        {

            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            await _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task UnloadScene()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task UnloadScenes()
        {

            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(0, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task PreloadScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task AddPreloadScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
        }

        [Test]
        public async Task SwitchPreloadScene()
        {

            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
        }

        [Test]
        public async Task ReActiveScene()
        {

            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task ReActivePreloadScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task ReAddUnloadScene()
        {

            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(3, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task Switch_NotDisableAgain()
        {

            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

    }
}