using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Tests;
using System.Threading.Tasks;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPageContainer
    {
        private IPageModule _pageContainer;

        [SetUp]
        public async Task SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());

            var gameLogic = new SampleGameLogic();
            gameLogic.Init();
            await gameLogic.Run();

            var _sceneModule = new SceneModule();
            gameLogic.GetContextInjector().Inject(_sceneModule);
            await _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
            var sceneNode = _sceneModule.TestGetViewByKey<SceneNode>(SampleSceneKeys.SampleSceneKeyOne);
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
        }


        [Test]
        public async Task PushPage()
        {
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

        public async Task SwitchPage()
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
    }
}
