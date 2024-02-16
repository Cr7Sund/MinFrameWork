using Cr7Sund.AssetLoader.Impl;
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


            // Local In GameNode or GameController
            // --- --- 
            // InjectionBinder.Bind<IPoolBinder>().To(new PoolBinder());
            InjectionBinder.Bind<ISceneLoader>().To(AssetLoaderFactory.CreateSceneLoader()).AsSingleton();

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
