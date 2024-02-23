using System;
using Cr7Sund.Package.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUITransitionAnimationContainer : IDisposable
    {
        IPromise<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation);
        IPromise<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter);
    }
}