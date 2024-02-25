using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIContext : CrossContext
    {
        public sealed override void AddComponents(INode node)
        {
            // Local In GameNode or GameController
            // --- --- 
            var assetLoader = AssetLoaderFactory.CreateLoader();
            var logger = InternalLoggerFactory.Create(LogChannel.UILogic);

            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.UILogger).AsCrossContext();

            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton().ToName(ServerBindDefine.UITimer);
            InjectionBinder.Bind<IAssetLoader>().To(assetLoader);
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IAssetLoader>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IPromiseTimer>(ServerBindDefine.UITimer);
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.UILogger);
        }

    }
}