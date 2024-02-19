using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIContext : CrossContext
    {
        public sealed override void AddComponents()
        {
            // Cross Context
            // --- --- 


            // Local In GameNode or GameController
            // --- --- 
            var assetLoader = AssetLoaderFactory.CreateLoader();
            assetLoader.Init();

            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsSingleton();
            base.InjectionBinder.Bind<IAssetLoader>().To(assetLoader).AsSingleton();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<IAssetLoader>();
            // InjectionBinder.Unbind<IPoolBinder>();
        }

    }
}