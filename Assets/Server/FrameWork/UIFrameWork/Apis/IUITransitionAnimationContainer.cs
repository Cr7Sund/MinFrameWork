using System.Threading;
using Cr7Sund.Server.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUITransitionAnimationContainer : IAssetContainer
    {
        PromiseTask<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation, UnsafeCancellationToken cancellation);
        PromiseTask<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter, UnsafeCancellationToken cancellation);
        PromiseTask UnloadAnimation(UITransitionAnimation animation);
    }
}