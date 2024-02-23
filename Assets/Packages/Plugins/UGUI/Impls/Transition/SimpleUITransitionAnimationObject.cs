using UnityEngine;

namespace Cr7Sund.UGUI.Impls
{
    [System.Serializable]
    public sealed class SimpleUITransitionAnimationObject : UITransitionAnimationObject
    {
        [SerializeField] private int _delay;
        [SerializeField] private int _duration = 300;
        [SerializeField] private EaseType _easeType = EaseType.QuarticEaseOut;
        [SerializeField] private SheetAlignment _beforeAlignment = SheetAlignment.Center;
        [SerializeField] private Vector3 _beforeScale = Vector3.one;
        [SerializeField] private float _beforeAlpha = 1.0f;
        [SerializeField] private SheetAlignment _afterAlignment = SheetAlignment.Center;
        [SerializeField] private Vector3 _afterScale = Vector3.one;
        [SerializeField] private float _afterAlpha = 1.0f;

        private Vector3 _afterPosition;
        private Vector3 _beforePosition;
        private CanvasGroup _canvasGroup;

        public override int Duration => _duration;

        public static SimpleUITransitionAnimationObject CreateInstance(int? duration = null, EaseType? easeType = null,
            SheetAlignment? beforeAlignment = null, Vector3? beforeScale = null, float? beforeAlpha = null,
            SheetAlignment? afterAlignment = null, Vector3? afterScale = null, float? afterAlpha = null)
        {
            var anim = CreateInstance<SimpleUITransitionAnimationObject>();
            anim.SetParams(duration, easeType, beforeAlignment, beforeScale, beforeAlpha, afterAlignment, afterScale,
                afterAlpha);
            return anim;
        }

        public override void Setup()
        {
            _beforePosition = _beforeAlignment.ToPosition(RectTransform);
            _afterPosition = _afterAlignment.ToPosition(RectTransform);
            if (!RectTransform.gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = RectTransform.gameObject.AddComponent<CanvasGroup>();
            }

            _canvasGroup = canvasGroup;
        }

        public override void SetTime(int time)
        {
            time = Mathf.Max(0, time - _delay);
            var progress = _duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / _duration);
            progress = Easings.Interpolate(progress, _easeType);
            var position = Vector3.Lerp(_beforePosition, _afterPosition, progress);
            var scale = Vector3.Lerp(_beforeScale, _afterScale, progress);
            var alpha = Mathf.Lerp(_beforeAlpha, _afterAlpha, progress);
            RectTransform.anchoredPosition = position;
            RectTransform.localScale = scale;
            _canvasGroup.alpha = alpha;
        }

        public void SetParams(int? duration = null, EaseType? easeType = null, SheetAlignment? beforeAlignment = null,
            Vector3? beforeScale = null, float? beforeAlpha = null, SheetAlignment? afterAlignment = null,
            Vector3? afterScale = null, float? afterAlpha = null)
        {
            if (duration.HasValue)
            {
                _duration = duration.Value;
            }

            if (easeType.HasValue)
            {
                _easeType = easeType.Value;
            }

            if (beforeAlignment.HasValue)
            {
                _beforeAlignment = beforeAlignment.Value;
            }

            if (beforeScale.HasValue)
            {
                _beforeScale = beforeScale.Value;
            }

            if (beforeAlpha.HasValue)
            {
                _beforeAlpha = beforeAlpha.Value;
            }

            if (afterAlignment.HasValue)
            {
                _afterAlignment = afterAlignment.Value;
            }

            if (afterScale.HasValue)
            {
                _afterScale = afterScale.Value;
            }

            if (afterAlpha.HasValue)
            {
                _afterAlpha = afterAlpha.Value;
            }
        }
    }
}