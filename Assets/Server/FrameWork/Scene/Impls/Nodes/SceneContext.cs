using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class SceneContext : CrossContext
    {
        public sealed override void AddComponents()
        {
            // Cross Context
            // --- --- 
            InjectionBinder.Bind<ISceneLoader>().To(AssetLoaderFactory.CreateSceneLoader());


            // Local In GameNode or GameController
            // --- --- 
            // InjectionBinder.Bind<IPoolBinder>().To(new PoolBinder());

            OnMappedBindings();
        }

        public sealed override void RemoveComponents()
        {
            InjectionBinder.Unbind<ISceneLoader>();
            // InjectionBinder.Unbind<IPoolBinder>();

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
