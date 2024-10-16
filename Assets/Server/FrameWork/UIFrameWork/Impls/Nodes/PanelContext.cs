using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.AssetContainers;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.IocContainer;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.UI.Impl
{
    public class PanelContext : CrossContext, INodeContext
    {
        protected virtual string Channel { get => "Panel"; }

        public void AddComponents(INode node)
        {
            var logger = InternalLoggerFactory.Create(Channel);

            // Local In GameNode or GameController
            // --- ---
            BindInstance<IInternalLog>(logger, ServerBindDefine.UILogger);
            BindInstance<IUINode>(node);
            BindAsSingleton<IPoolBinder, PoolBinder>();
            BindAsSingleton<IPromiseTimer, PromiseTimer>(ServerBindDefine.UITimer);
            BindAsSingleton<IPanelModule, PanelModule>();

            OnMappedBindings();
        }

        public void RemoveComponents()
        {
            Unbind<IInternalLog>(ServerBindDefine.UILogger);
            Unbind<IPoolBinder>();
            Unbind<IPromiseTimer>(ServerBindDefine.UITimer);
            Unbind<IUINode>();
            Unbind<IPanelModule>();

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
