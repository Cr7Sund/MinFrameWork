using Cr7Sund.Touch.Api;
using Cr7Sund.Touch.Impl;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using Cr7Sund.EventBus;
using Cr7Sund.Server.Apis;
using Cr7Sund.Framework.Tests;

namespace Cr7Sund.Server.Tests
{
    public class TestSceneModule
    {
        private ISceneModule _sceneModule;


        [SetUp]
        public void SetUp()
        {
            _sceneModule = new SceneModule();

            var gameNode = GameDirector.Construct<SampleGameBuilder>();


            var injectionBinder = new InjectionBinder();
            
            EventBus.EventBus eventBus = new EventBus.EventBus();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(new PoolBinder());
            injectionBinder.Bind<IFingerGesture>().To(new FingerGesture());
            injectionBinder.Bind<GameNode>().To(gameNode);
            injectionBinder.Bind<IEventBus>().To(eventBus);
            Debug.Init(new InternalLogger());

            injectionBinder.Injector.Inject(eventBus);
            injectionBinder.Injector.Inject(_sceneModule);

            gameNode.Run();
        }


        [Test]
        public void AddScene()
        {
            _sceneModule.AddScene(SampleSceneKeys.SampleSceneKeyOne);
        }
    }
}