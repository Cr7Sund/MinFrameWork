using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIContext : CrossContext
    {
        public sealed override void AddComponents()
        {
            // Local In GameNode or GameController
            // --- --- 
            var assetLoader = AssetLoaderFactory.CreateLoader();
            assetLoader.Init();

            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsSingleton();
            InjectionBinder.Bind<IAssetLoader>().To(assetLoader);
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IAssetLoader>();
            // InjectionBinder.Unbind<IPoolBinder>();
        }

    }
}