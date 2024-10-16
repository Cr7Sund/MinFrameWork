using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Impl;
using Cr7Sund.Touch.Api;
using Cr7Sund.Touch.Impl;
using Cr7Sund.Package.EventBus.Impl;
using Cr7Sund.AssetContainers;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.IocContainer;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.AssetContainers
{
    public abstract class GameContext : CrossContext, INodeContext
    {
        protected abstract string Channel { get; }

        public GameContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public void AddComponents(INode self)
        {
            var assetLoader = AssetLoaderFactory.CreateLoader();
            var logger = InternalLoggerFactory.Create(Channel);
            var sceneLoader = AssetLoaderFactory.CreateSceneLoader();

            // Cross Context
            // --- ---
            BindAsCrossAndSingleton<IFingerGesture, FingerGesture>();
            BindAsCrossAndSingleton<IEventBus, EventBus>();
            BindAsCrossAndSingleton<ISceneModule, SceneModule>();
            BindAsCrossAndSingleton<IConfigContainer, ConfigContainer>();
            BindAsCrossAndSingleton<IUITransitionAnimationContainer, UITransitionAnimationContainer>();
            BindAsCrossAndSingleton<IInstancesContainer, GameInstanceContainer>(ServerBindDefine.GameInstancePool);
            BindAsCrossAndSingleton<IInstancesContainer, UIPanelContainer>(ServerBindDefine.UIPanelContainer);
            BindAsCrossAndSingleton<IUniqueInstanceContainer, UIPanelUniqueContainer>(ServerBindDefine.UIPanelUniqueContainer);
            BindInstanceAsCrossContext<IAssetLoader>(assetLoader);
            BindInstanceAsCrossContext<ISceneLoader>(sceneLoader);
            BindInstanceAsCrossContext<IInternalLog>(logger, ServerBindDefine.GameLogger);

            // Local In GameNode or GameController
            // --- ---
            BindInstance<IGameNode>(self);
            BindAsSingleton<IPoolBinder, PoolBinder>();
            BindAsSingleton<IPromiseTimer, PromiseTimer>(ServerBindDefine.GameTimer);

            OnMappedBindings();
        }

        public void RemoveComponents()
        {
            Unbind<IFingerGesture>();
            Unbind<IEventBus>();
            Unbind<ISceneModule>();
            Unbind<IConfigContainer>();
            Unbind<IUITransitionAnimationContainer>();
            Unbind<IInstancesContainer>(ServerBindDefine.GameInstancePool);
            Unbind<IInstancesContainer>(ServerBindDefine.UIPanelContainer);
            Unbind<IUniqueInstanceContainer>(ServerBindDefine.UIPanelUniqueContainer);
            Unbind<IAssetLoader>();
            Unbind<ISceneLoader>();
            Unbind<IInternalLog>(ServerBindDefine.GameLogger);

            Unbind<IGameNode>();
            Unbind<IPoolBinder>();
            Unbind<IPromiseTimer>(ServerBindDefine.GameTimer);

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
