using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.UI.Impl
{
    public class UITransitionAnimationContainer : BaseAssetContainer, IUITransitionAnimationContainer
    {
        [Inject] private IConfigContainer _configContainer;
        [Inject] private IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;


        public async PromiseTask<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation)
        {
            switch (animation.AssetType)
            {
                case UIAnimationAssetType.MonoBehaviour:
                    return animation.AnimationBehaviour;
                case UIAnimationAssetType.ScriptableObject:
                    var asset = await base.LoadAssetAsync<Object>(animation.AnimationAsset)
                     as IPromise<IUITransitionAnimationBehaviour>;
                    return asset as IUITransitionAnimationBehaviour;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async PromiseTask<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter)
        {
            var settings = await _configContainer.LoadAssetAsync<UIScreenNavigatorSettings>(ConfigDefines.UITransitionConfig);
            return settings.GetDefaultPageTransitionAnimation(push, enter);
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
            Unload(ConfigDefines.UITransitionConfig);
            base.Dispose();
        }

    }
}