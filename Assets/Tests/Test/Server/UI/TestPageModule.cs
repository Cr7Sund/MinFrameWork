#pragma warning disable CS4014

using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Test;
using System.Threading.Tasks;
using UnityEngine;
using Cr7Sund.Server.Impl;
using Cr7Sund.Selector.Impl;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPageModule
    {
        private PageModule _pageContainer;
        private SampleGameLogic _gameLogic;
        private SceneModule _sceneModule;
        private TestGameRoot _gameRoot;

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
            _gameLogic.GetContextInjector().Inject(_sceneModule);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyTwo);
            var metGetNode = typeof(LoadModule).GetMethod("GetViewByKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genMetGetNode = metGetNode.MakeGenericMethod(typeof(SceneNode));
            var sceneNode = (SceneNode)genMetGetNode.Invoke(_sceneModule, new[] { SampleSceneKeys.SampleSceneKeyTwo });
            var sceneInjectBinder = sceneNode.Context.InjectionBinder;

            _pageContainer = new PageModule();
            sceneInjectBinder.Injector.Inject(_pageContainer);

            SampleOneUIController.Init();
            SampleTwoUIController.Init();
            SampleThreeUIController.Init();
            SampleFourUIController.Init();
            SampleFiveUIController.Init();
            SampleThreeUIController.promise = Promise.Resolved();
            SampleFivePanel.AnimPromise = Promise.Resolved();
            SampleThreeUIController.Rejected = false;
            SampleFivePanel.Rejected = false;
            SampleThreePanel.Rejected = false;
            SampleThreePanel.LoadPromise = Promise.Resolved();

            if (Application.isPlaying)
            {
                var gameRoot = new GameObject("testRoot", typeof(TestGameRoot));
                _gameRoot = gameRoot.GetComponent<TestGameRoot>();
                _gameRoot.Init(_gameLogic);
            }

        }

        [TearDown]
        public async Task TearDown()
        {
            await _pageContainer.CloseAll();
            await _sceneModule.UnloadScene(SampleSceneKeys.SampleSceneKeyTwo);
            await _gameLogic.DestroyAsync();
            if (Application.isPlaying)
            {
                GameObject.Destroy(_gameRoot.gameObject);
            }

        }

        [Test]
        public async Task TestPageModule_PushPage_IncreaseChildCount()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

        public async Task TestPageModule_SwitchPage_IncreaseChildCount()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
        }

        [Test]
        public async Task SwitchPages()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(3, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }
        [Test]
        public async Task SwitchPages_WithNoStack()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleFourUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task ExitBeforeSequence_Pending()
        {
            SampleThreeUIController.promise = Promise.Create();
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            var task = _pageContainer.PushPage(SampleUIKeys.SampleThreeUI); // sample three is hide first

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);

            SampleThreeUIController.promise.Resolve();
            await task;
        }

        [Test]
        public async Task ExitBeforeSequence_Resolved()
        {
            SampleThreeUIController.promise = Promise.Create();
            SampleThreeUIController.promise.Resolve();

            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.StartValue);
        }

        [Test]
        public async Task BackPage()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            await _pageContainer.BackPage();

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
        }

        [Test]
        public async Task BackPages()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            await _pageContainer.BackPage();

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task BackPageByPopCount()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            await _pageContainer.BackPage(2);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task BackPageByDest()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            await _pageContainer.BackPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task BackPage_Destroy()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            Assert.AreEqual(1, SampleFourUIController.StartValue);
            await _pageContainer.BackPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(0, SampleFourUIController.StartValue);
        }

        [Test]
        public async Task BackPages_Destroy()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);

            await _pageContainer.BackPage();

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleFourUIController.EnableCount);
        }

        [Test]
        public async Task BackPage_EmptyException()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            MyException ex = null;
            try
            {
                await _pageContainer.BackPage();
            }
            catch (MyException e)
            {
                ex = e;
            }

            Assert.AreEqual(UIExceptionType.NO_LEFT_BACK, ex.Type);

        }

        [Test]
        public async Task CloseAllPages()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);

            await _pageContainer.CloseAll();

            Assert.AreEqual(0, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleFourUIController.StartValue);
            Assert.AreEqual(0, SampleOneUIController.StartValue);
            Assert.AreEqual(0, SampleTwoUIController.StartValue);
        }

        [Test]
        public async Task CancelUI_Success()
        {
            SampleThreePanel.LoadPromise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            await _pageContainer.CancelNode(SampleUIKeys.SampleThreeUI);
            SampleThreePanel.LoadPromise.Cancel();// simulate  cancel load Promise


            Assert.AreEqual(0, SampleThreeUIController.StartValue);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task ReAddCancelUI()
        {
            SampleThreePanel.LoadPromise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            await _pageContainer.CancelNode(SampleUIKeys.SampleThreeUI);
            SampleThreePanel.LoadPromise.Cancel();// simulate  cancel load Promise

            SampleThreePanel.LoadPromise = new Promise();
            SampleThreePanel.LoadPromise.Resolve();
            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            Assert.AreEqual(1, SampleThreeUIController.StartValue);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }
    }
}
