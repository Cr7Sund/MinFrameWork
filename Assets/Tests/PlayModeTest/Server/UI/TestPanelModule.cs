using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Test;
using System.Threading.Tasks;
using Cr7Sund.Server.UI.Api;
using UnityEngine;
using Cr7Sund.AssetContainers;
using Cr7Sund.Selector.Impl;
using System.Text.RegularExpressions;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPanelModule
    {
        private IPanelModule _panelContainer;
        private TestGameRoot _gameRoot;
        private SceneModule _sceneModule;
        private SampleGameLogic _gameLogic;
        private PageModule _pageContainer;
        [SetUp]
        public async Task SetUp()
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
            SampleSceneOneController.Init();

            _gameLogic.GetContextInjector().Inject(_sceneModule);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            var metGetNode = typeof(LoadModule).GetMethod("GetViewByKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genMetGetNode = metGetNode.MakeGenericMethod(typeof(SceneNode));
            var sceneNode = (SceneNode)genMetGetNode.Invoke(_sceneModule, new[] { SampleSceneKeys.SampleSceneKeyOne });

            _pageContainer = new PageModule();
            sceneNode.Context.Inject(_pageContainer);


            SampleOneUIController.Init();
            SampleTwoUIController.Init();
            SampleThreeUIController.Init();
            SampleFourUIController.Init();
            SampleFiveUIController.Init();
            SampleThreeUIController.promise = Promise.Resolved();
            SampleFivePanel.AnimPromise = Promise.Resolved();
            SampleThreeUIController.Rejected = false;
            SampleFivePanel.Rejected = false;
            SampleThreePanel.LoadPromise = Promise.Resolved();
            SampleThreePanel.Rejected = false;

            if (Application.isPlaying)
            {
                var gameRoot = new GameObject("testRoot", typeof(TestGameRoot));
                _gameRoot = gameRoot.GetComponent<TestGameRoot>();
                _gameRoot.Init(_gameLogic);
            }

            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            genMetGetNode = metGetNode.MakeGenericMethod(typeof(UINode));
            var uiNode = (UINode)genMetGetNode.Invoke(_pageContainer, new[] { SampleUIKeys.SampleOneUI });

            _panelContainer = new PanelModule();
            uiNode.Context.Inject(_panelContainer);

            if (Application.isPlaying)
            {
                _gameRoot.PromiseTimer.Schedule((timeData) =>
                {
                    _panelContainer.TimeOut(timeData.elapsedTime);
                }, UnsafeCancellationToken.None);
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            await _panelContainer.CloseAll();
            await _pageContainer.CloseAll();
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
            await _gameLogic.DestroyAsync();

            if (Application.isPlaying)
            {
                GameObjectUtil.Destroy(_gameRoot.gameObject);
            }
        }


        [Test]
        public async Task OpenPanel()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
        }

        [Test]
        public async Task OpenPanels()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(3, _panelContainer.OperateNum);
            Assert.AreEqual(2, SampleOneUIController.EnableCount); // pageModule had open first
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task ExitBeforeSequence_Pending()
        {

            SampleThreeUIController.promise = Promise.Create();
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            var task = _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);

            SampleThreeUIController.promise.Resolve();
            await task;
        }

        [Test]
        public async Task ExitBeforeSequence_Resolved()
        {
            SampleThreeUIController.promise = Promise.Create();
            SampleThreeUIController.promise.Resolve();

            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(2, _panelContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.StartValue);
        }

        [Test]
        public async Task OpenPanelAndCloseOthers()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);
            await _panelContainer.OpenPanelAndCloseOthers(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }


        [Test]
        public async Task CloseAllPanels()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleFourUI);

            Assert.AreEqual(1, SampleFourUIController.StartValue);
            await _panelContainer.CloseAll();

            Assert.AreEqual(0, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleFourUIController.StartValue);
            Assert.AreEqual(0, SampleTwoUIController.StartValue);
        }

        [Test]
        public async Task OpenPanel_Pending_TimeOut()
        {
            AssertHelper.Expect(LogType.Error, new Regex("System.OperationCanceledException"));
            SampleThreeUIController.promise = Promise.Create();

            try
            {
                await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);
            }
            catch (System.Exception ex)
            {
                Console.Error(ex);
            }

            Assert.AreEqual(0, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task T()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
           
        }
    }
}
