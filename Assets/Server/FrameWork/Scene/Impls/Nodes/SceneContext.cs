using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;
using Cr7Sund.Server.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class SceneContext : CrossContext
    {
        protected abstract string Channel { get; }
        public sealed override void AddComponents(INode self)
        {
            var logger = InternalLoggerFactory.Create(Channel);
            var sceneContainer = new SceneInstanceContainer();
            sceneContainer.Init(self.Key.Key);

            // Cross Context
            // --- --- 
            InjectionBinder.Bind<ISceneInstanceContainer>().To(sceneContainer).AsCrossContext();

            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<ISceneNode>().To(self);
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.SceneTimer);
            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.SceneLogger);
            InjectionBinder.Bind<IPageModule>().To<PageModule>().AsSingleton().AsCrossContext();

            OnMappedBindings();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<ISceneInstanceContainer>();

            InjectionBinder.Unbind<ISceneNode>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.SceneTimer);
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.SceneLogger);
            InjectionBinder.Unbind<IPageModule>();

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
