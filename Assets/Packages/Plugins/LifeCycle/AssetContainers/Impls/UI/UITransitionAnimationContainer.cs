using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetContainers
{
    public class UITransitionAnimationContainer : BaseAssetContainer, IUITransitionAnimationContainer
    {
        [Inject] private IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;


        public async PromiseTask<IUITransitionAnimationBehaviour> GetAnimationBehaviour(UITransitionAnimation animation, UnsafeCancellationToken cancellation)
        {
            switch (animation.AssetType)
            {
                case UIAnimationAssetType.MonoBehaviour:
                    return animation.AnimationBehaviour;
                case UIAnimationAssetType.ScriptableObject:
                    var asset = await LoadAssetAsync<Object>(animation.AnimationAsset, cancellation);                  
                    return asset as IUITransitionAnimationBehaviour;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async PromiseTask<IUITransitionAnimationBehaviour> GetDefaultPageTransition(bool push, bool enter, UnsafeCancellationToken cancellation)
        {
            var settings = await LoadAssetAsync<UIScreenNavigatorSettings>(UIConfigDefines.UITransitionConfig, cancellation);
            return settings.GetDefaultPageTransitionAnimation(push, enter);
        }

        public async PromiseTask UnloadAnimation(UITransitionAnimation animation)
        {
            AssertUtil.NotNull(animation);

            switch (animation.AssetType)
            {
                case UIAnimationAssetType.ScriptableObject:
                   await Unload(animation.AnimationAsset);
                    break;
                case UIAnimationAssetType.MonoBehaviour:
                default:
                    break;
            }

        }

    }
}