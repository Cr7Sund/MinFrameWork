using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class SceneContext : CrossContext
    {
        protected abstract string Channel { get; }
        public sealed override void AddComponents(INode self)
        {
            var assetLoader = AssetLoaderFactory.CreateLoader();
            var sceneLoader = AssetLoaderFactory.CreateSceneLoader();
            var sceneContainer = new SceneContainer();
            sceneContainer.Init(self.Key.Key);
            var logger = InternalLoggerFactory.Create(Channel);

            // Cross Context
            // --- --- 
            InjectionBinder.Bind<ISceneContainer>().To(sceneContainer).AsCrossContext();

            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<ISceneNode>().To(self);
            InjectionBinder.Bind<ISceneLoader>().To(sceneLoader);
            InjectionBinder.Bind<IAssetLoader>().To(assetLoader);
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.SceneTimer);
            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.SceneLogger);

            OnMappedBindings();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<ISceneNode>();
            InjectionBinder.Unbind<ISceneLoader>();
            InjectionBinder.Unbind<IAssetLoader>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.SceneTimer);
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.SceneLogger);

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
