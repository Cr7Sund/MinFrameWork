using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Tests;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPageContainer
    {
        private SceneNode _sceneNode;
        private PageContainer _pageContainer;

        [SetUp]
        public void SetUp()
        {
            _pageContainer = new PageContainer();
            var builder = new SampleSceneOneBuilder();
            SceneDirector.Construct(builder, new SceneKey());
            _sceneNode = builder.GetProduct();

            var sceneInjectBinder = _sceneNode.Context.InjectionBinder;
            var gameContext = new SampleGameContext();
            gameContext.AddComponents(new GameNode());
            if (sceneInjectBinder is ICrossContextInjectionBinder childContextBinder)
            {
                var gameCrossBinder = gameContext.InjectionBinder as ICrossContextInjectionBinder;
                childContextBinder.CrossContextBinder = new CrossContextInjectionBinder();
                childContextBinder.CrossContextBinder.CopyFrom(gameCrossBinder.CrossContextBinder);
            }


            Console.Init(InternalLoggerFactory.Create());
            RunScene();

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

        private void RunScene()
        {
            _sceneNode.Inject();
            _sceneNode.Init();
            _sceneNode.Start();
            _sceneNode.Enable();
        }


       //[Test]
        public void PushPage()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

        public void SwitchPage()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
        }

       //[Test]
        public void SwitchPages()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);


            Assert.AreEqual(3, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }
       //[Test]
        public void SwitchPages_WithNoStack()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);


            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleFourUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }



       //[Test]
        public void ExitBeforeSequence_Pending()
        {
            SampleThreeUIController.promise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);
        }

       //[Test]
        public void ExitBeforeSequence_Resolved()
        {
            SampleThreeUIController.promise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);


            SampleThreeUIController.promise.Resolve();
            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.StartValue);
        }

       //[Test]
        public void BackPage()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            _pageContainer.BackPage();

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
        }

       //[Test]
        public void BackPages()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            _pageContainer.BackPage();

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

       //[Test]
        public void BackPageByPopCount()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            _pageContainer.BackPage(2);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

       //[Test]
        public void BackPageByDest()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            _pageContainer.BackPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

       //[Test]
        public void BackPage_Destroy()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleFourUI);

            Assert.AreEqual(1, SampleFourUIController.StartValue);
            _pageContainer.BackPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(0, SampleFourUIController.StartValue);
        }

       //[Test]
        public void BackPages_Destroy()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);

            _pageContainer.BackPage();

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleTwoUIController.EnableCount);
            Assert.AreEqual(0, SampleFourUIController.EnableCount);
        }

       //[Test]
        public void BackPage_EmptyException()
        {
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            TestDelegate testDelegate = () => _pageContainer.BackPage();
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(UIExceptionType.NO_LEFT_BACK, ex.Type);
        }

    }
}
