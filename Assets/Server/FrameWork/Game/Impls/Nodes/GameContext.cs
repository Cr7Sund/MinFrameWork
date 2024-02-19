using Cr7Sund.PackageTest.EventBus.Api;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Touch.Api;
using Cr7Sund.Touch.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class GameContext : CrossContext
    {
        private IEventBus _eventBus;
        private ISceneModule _sceneModule;

        public GameContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public sealed override void AddComponents()
        {
            // Cross Context
            // --- --- 
            _eventBus = new PackageTest.EventBus.Impl.EventBus();
            _sceneModule = new SceneModule();

            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IFingerGesture>().To<FingerGesture>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IEventBus>().To(_eventBus).AsSingleton().AsCrossContext();
            InjectionBinder.Bind<ISceneModule>().To(_sceneModule).AsSingleton().AsCrossContext();

            // Local In GameNode or GameController
            // --- --- 

            OnMappedBindings();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IEventBus>();
            InjectionBinder.Unbind<ISceneModule>();
            InjectionBinder.Unbind<IFingerGesture>();

            OnUnMappedBindings();
        }


        protected virtual void OnMappedBindings()
        {
        }
        protected virtual void OnUnMappedBindings()
        {
        }


    }
}
