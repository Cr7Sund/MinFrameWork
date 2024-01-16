using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class SceneContext : CrossContext
    {
        public SceneContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public sealed override void MapBindings()
        {
            // Cross Context
            // --- --- 


            // Local In GameNode or GameController
            // --- --- 
            OnMappedBindings();
        }

        public sealed override void UnMappedBindings()
        {
            InjectionBinder.Unbind<IPoolBinder>();

            OnUnMappedBindings();
        }

        protected abstract void OnMappedBindings();
        protected  abstract void OnUnMappedBindings();
    }
}
