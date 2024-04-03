using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.Server.UI.Impl
{
    public class UITransitionAnimationContainer : BaseAssetContainer, IUITransitionAnimationContainer
    {
        [Inject(ServerBindDefine.GameLoader)] IAssetLoader _assetLoader;
        [Inject] IConfigContainer _configContainer;
        private Dictionary<UITransitionAnimation, IAssetPromise> _dict = new();

        protected override IAssetLoader Loader => _assetLoader;


        public IPromise<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation)
        {
            switch (animation.AssetType)
            {
                case UIAnimationAssetType.MonoBehaviour:
                    return Promise<IUITransitionAnimationBehaviour>.Resolved(animation.AnimationBehaviour);
                case UIAnimationAssetType.ScriptableObject:
                    var assetPromise = LoadAssetAsync(animation.AnimationAsset);
                    return assetPromise
                               .Then(asset => asset as IUITransitionAnimationBehaviour);
                default:
                    return Promise<IUITransitionAnimationBehaviour>.Rejected(new ArgumentOutOfRangeException());
            }
        }

        public IPromise<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter)
        {
            return _configContainer.GetConfigAsync(ConfigDefines.UITransitionConfig)
                    .Then(asset =>
                    {
                        var setting = asset as UIScreenNavigatorSettings;
                        return setting.GetDefaultPageTransitionAnimation(push, enter);
                    });
        }

        public void UnloadAnimation(UITransitionAnimation animation)
        {
            AssertUtil.NotNull(animation);

            switch (animation.AssetType)
            {
                case UIAnimationAssetType.ScriptableObject:
                    Unload(animation.AnimationAsset);
                    break;
                case UIAnimationAssetType.MonoBehaviour:
                default:
                    break;
            }

        }

        public override void Dispose()
        {
            foreach (var item in _dict)
            {
                UnloadAnimation(item.Key);
            }

            _dict.Clear();

            base.Dispose();
        }

    }
}