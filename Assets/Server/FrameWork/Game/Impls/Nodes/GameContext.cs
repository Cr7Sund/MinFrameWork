using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Impl;
using Cr7Sund.Touch.Api;
using Cr7Sund.Touch.Impl;
using Cr7Sund.Package.EventBus.Impl;

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
            InjectionBinder.Bind<IFingerGesture>().To<FingerGesture>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IEventBus>().To<EventBus>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<ISceneModule>().To<SceneModule>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<PageContainer>().To<PageContainer>().AsSingleton().AsCrossContext();

            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();

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
