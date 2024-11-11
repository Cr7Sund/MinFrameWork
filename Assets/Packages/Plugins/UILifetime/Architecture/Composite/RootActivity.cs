using Cr7Sund.LifeTime;
using UnityEngine;
namespace Cr7Sund.LifeTime
{
    public class RootActivity : UpdatableNode, IUpdatable, ILateUpdatable
    {
        [Inject(UIConfigDefines.GameInstancePool)] private IInstancesContainer _gameContainer;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog Debug;
        
        private static readonly IRouteKey RouteKey = new RouteKey("RootActivity",
            typeof(RootActivity),
            typeof(RootActivityContext)
        );

        public RootActivity()
        {
            Init(RouteKey);
        }

        protected override PromiseTask OnCreateNode(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext)
        {
            return PromiseTask.CompletedTask;
        }

        protected async override PromiseTask OnNodeCreated(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext)
        {
            await _gameContainer.InstantiateAsync<Object>(UIConfigDefines.UIRootAssetKey,
                UIConfigDefines.UI_ROOT_NAME, cancellation);
        }
    }
}
