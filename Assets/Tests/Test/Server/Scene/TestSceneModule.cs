using Cr7Sund.Touch.Api;
using Cr7Sund.Touch.Impl;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Apis;
using Cr7Sund.Framework.Tests;
using Cr7Sund.EventBus.Api;
using Cr7Sund.EventBus.Impl;

namespace Cr7Sund.Server.Tests
{
    public class TestSceneModule
    {
        private ISceneModule _sceneModule;
        private GameNode _gameNode;

        [SetUp]
        public void SetUp()
        {
            _sceneModule = new SceneModule();

            _gameNode = GameDirector.Construct<SampleGameBuilder>();
            var injectionBinder = new InjectionBinder();
            var eventBus = new Cr7Sund.EventBus.Impl.EventBus();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(new PoolBinder());
            injectionBinder.Bind<IFingerGesture>().To(new FingerGesture());
            injectionBinder.Bind<GameNode>().To(_gameNode);
            injectionBinder.Bind<IEventBus>().To(eventBus);
            Debug.Init(new InternalLogger());

            injectionBinder.Injector.Inject(eventBus);
            injectionBinder.Injector.Inject(_sceneModule);

            SampleSceneOneController.Init();
            SampleSceneTwoController.Init();
        }


        [Test]
        public void AddScene()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void SwitchToFistScene()
        {
            _gameNode.Run();

            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void SwitchToNewScene()
        {
            _gameNode.Run();

            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void AddScenes()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void RemoveScene()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void RemoveScenes()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.RemoveScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(0, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }


        [Test]
        public void UnloadScene()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public void UnloadScenes()
        {
            _gameNode.Run();

            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(0, SampleSceneTwoController.StartValue);
            Assert.AreEqual(0, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public void PreloadScene()
        {
            _gameNode.Run();

            var preloadPromise = _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(PromiseState.Resolved, preloadPromise.CurState);
        }

        [Test]
        public void AddPreloadScene()
        {
            _gameNode.Run();

            _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);


            Assert.AreEqual(1, SampleSceneOneController.StartValue);
        }

        [Test]
        public void SwitchPreloadScene()
        {
            _gameNode.Run();

            _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);


            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
        }
    }
}