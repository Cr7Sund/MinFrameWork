using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.Tests;

namespace Cr7Sund.ServerTest.Scene
{
    public class TestSceneModule
    {
        private ISceneModule _sceneModule;
        private GameNode _gameNode;

        [SetUp]
        public void SetUp()
        {
            // InternalLog.Warn(LogChannel.Lua,)

            // Logger.Warn(LogChannel.Lua,)

            // Debug // do nothing


            _sceneModule = new SceneModule();

            _gameNode = GameDirector.Construct<SampleGameBuilder>();
            var injectionBinder = _gameNode.Context.InjectionBinder;

            Console.Init(InternalLoggerFactory.Create());
            _gameNode.Run();

            injectionBinder.Injector.Inject(_sceneModule);

            SampleSceneOneController.Init();
            SampleSceneTwoController.Init();

        }


        [Test]
        public void AddScene()
        {
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne)
                                    .Then(node => Assert.AreEqual(NodeState.Ready, node.NodeState));

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void SwitchToFistScene()
        {
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne)
                                    .Then(node => Assert.AreEqual(NodeState.Ready, node.NodeState));

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void SwitchToNewScene()
        {

            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void AddScenes()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void RemoveScene()
        {
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne)
                                    .Then(node => Assert.AreEqual(NodeState.Removed, node.NodeState));

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void RemoveScenes()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }


        [Test]
        public void UnloadScene()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void UnloadScenes()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(0, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void PreloadScene()
        {


            var preloadPromise = _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);

            Assert.AreEqual(PromiseState.Resolved, preloadPromise.CurState);
        }

        [Test]
        public void AddPreloadScene()
        {

            _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
        }

        [Test]
        public void SwitchPreloadScene()
        {


            _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);


            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
        }

        [Test]
        public void ReActiveScene()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void ReActivePreloadScene()
        {
            _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void ReAddUnloadScene()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(3, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void Switch_NotDisableAgain()
        {


            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }


    }
}