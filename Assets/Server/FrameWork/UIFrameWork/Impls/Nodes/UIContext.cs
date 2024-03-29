using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIContext : CrossContext
    {
        protected virtual string Channel { get => "UI"; }

        
        public sealed override void AddComponents(INode node)
        {
            // Local In GameNode or GameController
            // --- --- 
            var assetLoader = AssetLoaderFactory.CreateLoader();
            var logger = InternalLoggerFactory.Create(Channel);

            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.UILogger).AsCrossContext();

            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.UITimer);
            InjectionBinder.Bind<IAssetLoader>().To(assetLoader);
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.UILogger);

            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.UITimer);
            InjectionBinder.Unbind<IAssetLoader>();
        }

    }
}