using Cr7Sund.AssetContainers;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using UnityEngine;
namespace Cr7Sund.UILifeTime
{
    public abstract class UIView : IUIView
    {
        private readonly IAssetKey _partnerUIKey;

        public UIPanel _uiPanel;
        public CanvasGroup _canvasGroup;
        public Canvas _canvas;
        private RectTransform _attachPoint;
        private RectTransform _rectTransform;
        private string _identifier;
        [Inject(UIConfigDefines.UIPanelContainer)] private IInstancesContainer _uiContainer;

        protected abstract IAssetKey _uiKey { get; }


        public async PromiseTask Load(string panelID, UnsafeCancellationToken cancellationToken, RectTransform attachPoint)
        {
            var instance = await _uiContainer.InstantiateAsync<GameObject>(_uiKey, panelID, cancellationToken);
            OnCreate(instance, attachPoint);
        }

        private void OnCreate(GameObject go, RectTransform attachPoint)
        {
            if (!MacroDefine.IsMainThread || !UnityEngine.Application.isPlaying) return;

            _uiPanel = go.GetComponent<UIPanel>();
            _identifier = go.name.Replace("(Clone)", string.Empty);
            _attachPoint = attachPoint;

            _rectTransform = (RectTransform)go.transform;
            _canvasGroup = _uiPanel.GetUIComponent<CanvasGroup>(nameof(CanvasGroup));
            _canvas = _uiPanel.GetUIComponent<Canvas>(nameof(Canvas));

            // _rectTransform.FillParent(_attachPoint);
            // _rectTransform.AlignLeft(_attachPoint);
            _rectTransform.SetParent(_attachPoint);
        }


        public void Enable()
        {
            SortOrderView();
        }

        private void SortOrderView()
        {

        }

        public RectTransform FindAttachPoint(string name)
        {
            var attachPoint = _uiPanel.GetUIComponent<RectTransform>(name);
            if (attachPoint == null)
            {
                attachPoint = _rectTransform;
            }
            return attachPoint;
        }

        public bool IsValid() => _rectTransform != null;


    }

    public class UITransition : IFragmentTransition
    {
        [Inject] private IUITransitionAnimationContainer _animationContainer;
        [Inject] private IPromiseTimer _parentTimer;
        private CanvasGroup _canvasGroup=>_uiView._canvasGroup;
        private RectTransform _rectTransform=>_rectTransform;
        private UIPanel _uiPanel => _uiView._uiPanel;
        private readonly UIView _uiView;
        
        public float TransitionAnimationProgress { get; private set; }

                #region Transition
        public PromiseTask pop_enter(string partnerPage, UnsafeCancellationToken cancellationToken)
        {
            return EnterRoutine(false, partnerPage, cancellationToken);
        }
        public PromiseTask pop_exit(string partnerPage, UnsafeCancellationToken cancellationToken)
        {
            return ExitRoutine(false, partnerPage, cancellationToken);
        }
        public PromiseTask push_enter(string partnerPage, UnsafeCancellationToken cancellationToken)
        {
            return EnterRoutine(true, partnerPage, cancellationToken);
        }
        public PromiseTask push_exit(string partnerPage, UnsafeCancellationToken cancellationToken)
        {
            return ExitRoutine(true, partnerPage, cancellationToken);
        }

        private async PromiseTask EnterRoutine(bool push, string partnerPage, UnsafeCancellationToken cancellation)
        {
            if (_rectTransform == null)
            {
                return;
            }

            _canvasGroup.alpha = 1.0f;
            await TransitionRoutine(push, true, partnerPage, cancellation);
        }

        private async PromiseTask ExitRoutine(bool push, string partnerPage, UnsafeCancellationToken cancellation)
        {
            if (_rectTransform == null)
            {
                return;
            }

            await TransitionRoutine(push, false, partnerPage, cancellation);
            _canvasGroup.alpha = 0.0f;
        }

        private async PromiseTask TransitionRoutine(bool push, bool enter, string partnerRoute, UnsafeCancellationToken cancellation)
        {
            UITransitionAnimation animation = _uiPanel.GetAnimation(push, enter, partnerRoute);
            if (animation == null)
            {
                var transitionBehaviour = await _animationContainer.GetDefaultPageTransition(push, enter, cancellation);
                await PlayTransition(transitionBehaviour, cancellation);
                return;
            }

            {
                var transitionBehaviour = await _animationContainer.GetAnimationBehaviour(animation, cancellation);
                await PlayTransition(transitionBehaviour, cancellation);
            }
        }

        private async PromiseTask PlayTransition(IUITransitionAnimationBehaviour transitionBehaviour, UnsafeCancellationToken cancellation)
        {
            if (transitionBehaviour.Duration > 0.0f)
            {
                transitionBehaviour.Setup(_rectTransform);
                // PLAN replace with promise task
                // 🤔🤔🤔why we use scene(Game) timer?
                // since remove node will be happened in transition
                var promise = _parentTimer.Schedule(transitionBehaviour.Duration, (timeData) => transitionBehaviour.SetTime(timeData.elapsedTime), cancellation)
                    .Progress(SetTransitionProgress);

                // promise.Catch(ex => Console.Info("ExitAnim {ex}", ex));
                await promise.Task;
            }
        }

        private void SetTransitionProgress(float progress)
        {
            TransitionAnimationProgress = progress;
        }
        #endregion

    }

}
