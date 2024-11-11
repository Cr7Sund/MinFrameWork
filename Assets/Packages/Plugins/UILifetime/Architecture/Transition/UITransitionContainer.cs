using System;
using Cr7Sund.LifeTime;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.UGUI.Impls;
namespace Cr7Sund.LifeTime
{
    public class UITransitionContainer : ITransitionContainer
    {
        [Inject] IUniqueInstanceContainer _instancesContainer;

        public ITransition GetTransition(IRouteKey assetKey)
        {
            var panel = _instancesContainer.GetInstance<UIPanel>(assetKey);
            AssertUtil.NotNull(panel);
            
            return panel.GetComponent<TweenerTransition>();

            // var transitionAssetType = GameConfig.TransitionAssetType;
            // switch (transitionAssetType)
            // {
            //     case TransitionAssetType.Tweener:
            //         return _instancesContainer.GetInstance(assetKey).GetUI<TweenerTransition>();
            //     default:
            //         throw new NotImplementedException();
            // }
        }
    }

}
