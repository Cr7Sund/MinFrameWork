using System;
using Cr7Sund.Package.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUITransitionAnimationContainer : IDisposable
    {
        PromiseTask<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation);
        PromiseTask<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter);
        void UnloadAnimation(UITransitionAnimation animation);
    }
}