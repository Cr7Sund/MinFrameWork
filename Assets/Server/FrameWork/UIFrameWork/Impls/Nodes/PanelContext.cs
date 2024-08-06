using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PanelContext : CrossContext
    {
        protected virtual string Channel { get => "Panel"; }


        public sealed override void AddComponents(INode node)
        {
            var logger = InternalLoggerFactory.Create(Channel);

            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.UILogger).AsCrossContext();
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.UITimer);
            InjectionBinder.Bind<IUINode>().To(node);
            InjectionBinder.Bind<IPanelModule>().To<PanelModule>().AsSingleton();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.UILogger);
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.UITimer);
            InjectionBinder.Unbind<IUINode>();
            InjectionBinder.Unbind<IPanelModule>();
        }

    }
}