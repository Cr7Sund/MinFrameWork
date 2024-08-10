using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.IocContainer;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class SceneContext : CrossContext, INodeContext
    {
        protected abstract string Channel { get; }

        public void AddComponents(INode self)
        {
            var logger = InternalLoggerFactory.Create(Channel);
            var sceneContainer = new SceneInstanceContainer();
            sceneContainer.Init(self.Key.Key);

            // Cross Context
            // --- ---
            BindInstanceAsCrossContext<ISceneInstanceContainer>(sceneContainer);
            BindAsCrossAndSingleton<IPromiseTimer, PromiseTimer>(ServerBindDefine.SceneTimer);
            BindAsCrossAndSingleton<IPageModule, PageModule>();

            // Local In GameNode or GameController
            // --- ---
            BindInstance<ISceneNode>(self);
            BindAsSingleton<IPoolBinder, PoolBinder>();
            BindInstance<IInternalLog>(logger, ServerBindDefine.SceneLogger);

            OnMappedBindings();
        }

        public void RemoveComponents()
        {
            Unbind<ISceneInstanceContainer>();

            Unbind<ISceneNode>();
            Unbind<IPoolBinder>();
            Unbind<IPromiseTimer>(ServerBindDefine.SceneTimer);
            Unbind<IInternalLog>(ServerBindDefine.SceneLogger);
            Unbind<IPageModule>();

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
