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
using Cr7Sund.Server.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.Impl
{
    public abstract class GameContext : CrossContext
    {
        protected abstract string Channel { get; }
        public GameContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public sealed override void AddComponents(INode self)
        {
            var assetLoader = AssetLoaderFactory.CreateLoader();
            var logger = InternalLoggerFactory.Create(Channel);

            // Cross Context
            // --- --- 
            InjectionBinder.Bind<IFingerGesture>().To<FingerGesture>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IEventBus>().To<EventBus>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<ISceneModule>().To<SceneModule>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<PageContainer>().To<PageContainer>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IConfigContainer>().To<ConfigContainer>().AsSingleton().AsCrossContext();
            InjectionBinder.Bind<IUITransitionAnimationContainer>().To<UITransitionAnimationContainer>().AsSingleton().AsCrossContext();

            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<IGameNode>().To(self);
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IAssetLoader>().To(assetLoader);
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.GameTimer);
            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.GameLogger);

            OnMappedBindings();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IFingerGesture>();
            InjectionBinder.Unbind<IEventBus>();
            InjectionBinder.Unbind<ISceneModule>();
            InjectionBinder.Unbind<PageContainer>();
            InjectionBinder.Unbind<IConfigContainer>();
            InjectionBinder.Unbind<IUITransitionAnimationContainer>();

            InjectionBinder.Unbind<IGameNode>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IAssetLoader>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.GameTimer);
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.GameLogger);

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
