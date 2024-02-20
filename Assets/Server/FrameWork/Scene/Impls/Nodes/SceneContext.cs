using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class  SceneContext : CrossContext
    {
        public sealed override void AddComponents()
        {
            // Local In GameNode or GameController
            // --- --- 
            InjectionBinder.Bind<ISceneLoader>().To(AssetLoaderFactory.CreateSceneLoader()).AsSingleton();
            InjectionBinder.Bind<IPoolBinder>().To(new PoolBinder()).AsSingleton();

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


        public void Test_CreateCrossContext()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }
    }
}
