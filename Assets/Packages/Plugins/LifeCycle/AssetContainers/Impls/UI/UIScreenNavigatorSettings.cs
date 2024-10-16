using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using UnityEngine;

namespace Cr7Sund.AssetContainers
{
    [CreateAssetMenu(menuName = "Cr7Sund/UI/ScreenNavigation")]
    public sealed class UIScreenNavigatorSettings : ScriptableObject
    {
        [SerializeField] private UITransitionAnimationObject _pagePushEnterAnimation;

        [SerializeField] private UITransitionAnimationObject _pagePushExitAnimation;

        [SerializeField] private UITransitionAnimationObject _pagePopEnterAnimation;

        [SerializeField] private UITransitionAnimationObject _pagePopExitAnimation;

        public UITransitionAnimationObject PagePushEnterAnimation { get => _pagePushEnterAnimation; set => _pagePushEnterAnimation = value; }
        public UITransitionAnimationObject PagePushExitAnimation { get => _pagePushExitAnimation; set => _pagePushExitAnimation = value; }
        public UITransitionAnimationObject PagePopEnterAnimation { get => _pagePopEnterAnimation; set => _pagePopEnterAnimation = value; }
        public UITransitionAnimationObject PagePopExitAnimation { get => _pagePopExitAnimation; set => _pagePopExitAnimation = value; }

        public IUITransitionAnimationBehaviour GetDefaultPageTransitionAnimation(bool push, bool enter)
        {
            if (push)
            {
                return enter ? _pagePushEnterAnimation : _pagePushExitAnimation;
            }

            return enter ? _pagePopEnterAnimation : _pagePopExitAnimation;
        }
    }
}
