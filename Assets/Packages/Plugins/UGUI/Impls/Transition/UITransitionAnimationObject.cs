using Cr7Sund.UGUI.Apis;
using UnityEngine;

namespace Cr7Sund.UGUI.Impls
{
    /// <summary>
    ///     Base class for transition animation with ScriptableObject.
    /// </summary>
    public abstract class UITransitionAnimationObject : ScriptableObject, IUITransitionAnimationBehaviour
    {
        public RectTransform RectTransform { get; private set; }
        public RectTransform PartnerRectTransform { get; private set; }
        public abstract int Duration { get; }

        void IUITransitionAnimationBehaviour.SetPartner(RectTransform partnerRectTransform)
        {
            PartnerRectTransform = partnerRectTransform;
        }

        void IUITransitionAnimationBehaviour.Setup(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
            Setup();
            SetTime(0);
        }

        public abstract void SetTime(int time);

        public abstract void Setup();
    }
}