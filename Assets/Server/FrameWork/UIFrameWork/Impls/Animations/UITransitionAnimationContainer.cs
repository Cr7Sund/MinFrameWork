using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.Server.UI.Impl
{
    public class UITransitionAnimationContainer : IUITransitionAnimationContainer
    {
        [Inject] IAssetLoader _assetLoader;
        [Inject] IConfigContainer _configModule;
        private Dictionary<UITransitionAnimation, IAssetPromise> _dict = new();


        public IPromise<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation)
        {
            switch (animation.AssetType)
            {
                case UIAnimationAssetType.MonoBehaviour:
                    return Promise<IUITransitionAnimationBehaviour>.Resolved(animation.AnimationBehaviour);
                case UIAnimationAssetType.ScriptableObject:
                    var assetPromise = _assetLoader.InstantiateAsync(animation.AnimationAsset);
                    return assetPromise
                                       .Then(asset => asset as IUITransitionAnimationBehaviour);
                default:
                    return Promise<IUITransitionAnimationBehaviour>.Rejected(new ArgumentOutOfRangeException());
            }
        }

        public IPromise<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter)
        {
            return _configModule.GetConfigAsync<UIScreenNavigatorSettings>(ConfigDefines.UITransitionConfig)
                    .Then(setting => setting.GetDefaultPageTransitionAnimation(push, enter));
        }

        public void UnloadAnimation(UITransitionAnimation animation)
        {
            _assetLoader.ReleaseInstance(_dict[animation]);
        }

        public void Dispose()
        {
            foreach (var item in _dict)
            {
                UnloadAnimation(item.Key);
            }

            _dict.Clear();
        }

    }
}