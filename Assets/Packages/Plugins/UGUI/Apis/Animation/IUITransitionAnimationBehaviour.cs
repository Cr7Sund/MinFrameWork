using UnityEngine;

namespace Cr7Sund.UGUI.Apis
{
    public interface IUITransitionAnimationBehaviour : IUIAnimation
    {
        void SetPartner(RectTransform partnerRectTransform);

        void Setup(RectTransform rectTransform);
    }
}