using Cr7Sund.TweenTimeLine;
using PrimeTween;
using UnityEngine.EventSystems;
namespace Cr7Sund.LifeTime
{
    public abstract class TweenerTransition : UIBehaviour, ITransition, ITweener
    {
        [Inject]
        private TransConfig _transConfig;

        public async PromiseTask Show(IRouteKey targetPage, string partnerPage, bool push)
        {
            var transitionConfig = _transConfig.GetConfig(targetPage.Key, partnerPage);
            await Play(push ?
                transitionConfig.pushEnterAnimations
                : transitionConfig.popEnterAnimations);
        }

        public async PromiseTask Hide(IRouteKey targetPage, string partnerPage, bool push)
        {
            var transitionConfig = _transConfig.GetConfig(targetPage.Key, partnerPage);
            await Play(push ?
                transitionConfig.pushExitAnimations
                : transitionConfig.popExitAnimations);
        }

        public abstract Sequence Play(string tweenName);
    }
}
