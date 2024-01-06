using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public class GameContext : CrossContext
    {
        public GameContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public sealed override void MapBindings()
        {
            // Cross Context
            // --- --- 
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext();

            // Local In GameNode or GameController
            OnMappedBindings();
        }

        public sealed override void UnMappedBindings()
        {
            InjectionBinder.Unbind<IPoolBinder>();

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
