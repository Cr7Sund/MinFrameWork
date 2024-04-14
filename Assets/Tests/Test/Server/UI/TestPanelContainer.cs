using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Impl;
using NUnit.Framework;
using Cr7Sund.Server.Tests;
using System.Threading.Tasks;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPanelContainer
    {
        private IPanelModule _panelContainer;


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

            var pageContainer = new PageModule();
            sceneInjectBinder.Injector.Inject(pageContainer);
            await pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            var uiNode = pageContainer.TestGetViewByKey<UINode>(SampleUIKeys.SampleOneUI);
            var uiInjectBinder = uiNode.Context.InjectionBinder;

            _panelContainer = new PanelModule();
            uiInjectBinder.Injector.Inject(_panelContainer);

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
        public async Task OpenPanel()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

        [Test]
        public async Task OpenPages()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(3, _panelContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task ExitBeforeSequence_Pending()
        {
            SampleThreeUIController.promise = Promise.Create();
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            var task = _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);

            SampleThreeUIController.promise.Resolve();
            await task;
        }

        [Test]
        public async Task ExitBeforeSequence_Resolved()
        {
            SampleThreeUIController.promise = Promise.Create();
            SampleThreeUIController.promise.Resolve();

            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(2, _panelContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleThreeUIController.StartValue);
        }

        [Test]
        public async Task OpenPanelAndCloseOthers()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleTwoUI);
            await _panelContainer.OpenPanelAndCloseOthers(SampleUIKeys.SampleTwoUI);

            Assert.AreEqual(1, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleTwoUIController.EnableCount);
        }


        [Test]
        public async Task CloseAllPanels()
        {
            await _panelContainer.OpenPanel(SampleUIKeys.SampleOneUI);
            await _panelContainer.OpenPanel(SampleUIKeys.SampleFourUI);
          
            Assert.AreEqual(1, SampleFourUIController.StartValue);
            await _panelContainer.CloseAll();

            Assert.AreEqual(0, _panelContainer.OperateNum);
            Assert.AreEqual(0, SampleFourUIController.StartValue);
            Assert.AreEqual(0, SampleOneUIController.StartValue);
        }

    }
}
