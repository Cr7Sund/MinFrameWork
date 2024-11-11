using NUnit.Framework;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.Test;
using System.Threading.Tasks;
using UnityEngine;
using Cr7Sund.ServerTest.UI;
using Cr7Sund.Selector.Impl;
using Cr7Sund.AssetContainers;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Impl;
using System.Text.RegularExpressions;

namespace Cr7Sund.ServerTest.Scene
{
    public class TestSceneModule
    {
        private SceneModule _sceneModule;
        private SampleGameLogic _gameLogic;
        private TestGameRoot _gameRoot;

        [SetUp]
        public async Task TestSceneModule_SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());

            if (Application.isPlaying)
            {
                await GameMgr.Instance.Close();
            }

            _gameLogic = new SampleGameLogic();
            _gameLogic.Init();
            await _gameLogic.Run();

            _sceneModule = new SceneModule();
            _gameLogic.GetContextInjector().Inject(_sceneModule);

            SampleSceneOneController.Init();
            SampleSceneTwoController.Init();

            if (Application.isPlaying)
            {
                var gameRoot = new GameObject("testRoot", typeof(TestGameRoot));
                GameObject.DontDestroyOnLoad(gameRoot);
                _gameRoot = gameRoot.GetComponent<TestGameRoot>();
                _gameRoot.Init(_gameLogic);
                _gameRoot.PromiseTimer.Schedule((timeData) =>
                {
                    _sceneModule.TimeOut(timeData.elapsedTime);
                }, UnsafeCancellationToken.None);
            }
        }

        [TearDown]
        public async Task TestSceneModule_TearDown()
        {
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyTwo);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyThree);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyFour);

            _sceneModule.Dispose();

            await _gameLogic.DestroyAsync();
            if (Application.isPlaying)
            {
                GameObjectUtil.Destroy(_gameRoot.gameObject);
            }
        }

        [Test]
        public async Task TestSceneModule_AddScene_ShouldIncreaseSceneCount()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_AddScenesAsync_ShouldChangeState()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_UnloadScene()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_UnloadAllScenes()
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
        public async Task TestSceneModule_PreloadScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(0, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_AddPreloadSingleScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(2, SampleSceneOneController.StartValue);
        }

        [Test]
        public async Task TestSceneModule_AddPreloadAdditiveScene()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyThree);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyThree);
            Assert.AreEqual(2, SampleSceneOneController.StartValue);
        }

        [Test]
        public async Task TestSceneModule_PreloadAdditiveScenes_CloseOthers()
        {
            var metGetNode = typeof(LoadModule).GetMethod("GetViewByKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genMetGetNode = metGetNode.MakeGenericMethod(typeof(SceneNode));

            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyThree);
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyFour);

            var sceneThreeNode = (SceneNode)genMetGetNode.Invoke(_sceneModule, new[] { SampleSceneKeys.SampleSceneKeyThree });
            var sceneFourNode = (SceneNode)genMetGetNode.Invoke(_sceneModule, new[] { SampleSceneKeys.SampleSceneKeyFour });
            Assert.IsNull(sceneThreeNode);
            Assert.AreEqual(LoadState.Loaded, sceneFourNode.LoadState);
            Assert.AreEqual(NodeState.Preloaded, sceneFourNode.NodeState);
        }

        [Test]
        public async Task TestSceneModule_ReActivePreloadScene_OnlyEnableOnce()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_ReActiveScene_OnlyEnableOnce()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_ReAddUnloadScene_OnlyEnableOnce()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            Assert.AreEqual(3, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_SwitchToFistScene_ShouldChangeState()
        {
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyThree);

            Assert.AreEqual(2, SampleSceneOneController.StartValue);
            Assert.AreEqual(1, SampleSceneOneController.EnableCount);
        }
        

        [Test]
        public async Task TestSceneModule_SwitchToNewScene_ShouldCloseOtherScene()
        {
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyThree);
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyFour);

            Assert.AreEqual(1, SampleSceneOneController.StartValue);
            Assert.AreEqual(0, SampleSceneOneController.EnableCount);
            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
            Assert.AreEqual(1, SampleSceneTwoController.EnableCount);
        }

        [Test]
        public async Task TestSceneModule_SwitchFromPreloadScene_ShouldChangeState()
        {
            await _sceneModule.PreLoadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _sceneModule.SwitchScene(SampleSceneKeys.SampleSceneKeyTwo);

            Assert.AreEqual(1, SampleSceneTwoController.StartValue);
        }

        [Test]
        public async Task OpenScene_Pending_TimeOut()
        {
            AssertHelper.Expect(LogType.Error, new Regex("System.OperationCanceledException"));
            SampleSceneOneController.LoadPromise = Promise.Create();

            try
            {
                await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            }
            catch (System.Exception ex)
            {
                Console.Error(ex);
            }

            Assert.AreEqual(0, _sceneModule.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

    }
}
