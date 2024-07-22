using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using UnityEngine;
using Cr7Sund.Server.Utils;
using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Impl;
using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIView : IUIView
    {
        [Inject(ServerBindDefine.SceneTimer)] private IPromiseTimer _sceneTimer;
        [Inject] private IUITransitionAnimationContainer _animationContainer;
        [Inject(ServerBindDefine.GameInstancePool)] private IInstancesContainer _gameContainer;
        private int _sortingOrder;
        private UIPanel _uiPanel;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;
        private string _identifier;
        private Action<float> _transitionProgressReporter;
        private event Action<float> TransitionAnimationProgressChanged;
        private Dictionary<UITransitionAnimation, IUITransitionAnimationBehaviour> _transitionDict = new(4);

        private Action<float> TransitionProgressReporter
        {
            get
            {
                if (_transitionProgressReporter == null)
                    _transitionProgressReporter = SetTransitionProgress;
                return _transitionProgressReporter;
            }
        }
        public int SortingOrder { get => _sortingOrder; }
        public RectTransform RectTransform { get => _rectTransform; }
        public float TransitionAnimationProgress { get; private set; }

        public virtual PromiseTask OnLoad(GameObject go)
        {
            if (!MacroDefine.IsMainThread || !UnityEngine.Application.isPlaying) return PromiseTask.CompletedTask;

            //Instantiate
            AssertUtil.IsNull(_rectTransform, UIExceptionType.instantiate_UI_repeat);

            _uiPanel = go.GetComponent<UIPanel>();
            _identifier = go.name.Replace("(Clone)", string.Empty);

            _rectTransform = (RectTransform)go.transform;
            _canvasGroup = _uiPanel.GetUIComponent<CanvasGroup>(nameof(CanvasGroup));
            _canvas = _uiPanel.GetUIComponent<Canvas>(nameof(Canvas));

            return PromiseTask.CompletedTask;
        }

        public void Start(INode parent)
        {
            AssertUtil.NotNull(parent, UIExceptionType.null_UI_parent);
            _parentTransform = GetParentTrans(parent);
        }

        public void Enable(INode parent)
        {
            SortOrderView(parent);
        }

        public void BeforeEnter()
        {
            if (_rectTransform == null)
            {
                return;
            }

            // Set order of rendering.

            SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);
            _canvasGroup.alpha = 0.0f;
        }

        public virtual async PromiseTask EnterRoutine(bool push, IUINode partnerPage, bool playAnimation, UnsafeCancellationToken cancellation)
        {
            if (_rectTransform == null)
            {
                return;
            }

            _canvasGroup.alpha = 1.0f;

            if (playAnimation)
            {
                await TransitionRoutine(push, true, partnerPage, cancellation);
            }
        }

        public void AfterEnter()
        {
            if (_rectTransform == null)
            {
                return;
            }

            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(1.0f);
        }

        public void BeforeExit()
        {
            if (_rectTransform == null)
            {
                return;
            }

            SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1.0f;
        }

        public async PromiseTask ExitRoutine(bool push, IUINode partnerPage, bool playAnimation, UnsafeCancellationToken cancellation)
        {
            if (_rectTransform == null)
            {
                return;
            }

            if (playAnimation)
            {
                await TransitionRoutine(push, false, partnerPage, cancellation);
            }
        }

        public void AfterExit()
        {
            if (_rectTransform == null)
            {
                return;
            }

            _canvasGroup.alpha = 0.0f;
            SetTransitionProgress(1.0f);
            SetActive(false);
        }

        public void Disable()
        {
        }

        public void Stop()
        {
            foreach (var item in _transitionDict)
            {
                _animationContainer.UnloadAnimation(item.Key);
            }

            _transitionDict.Clear();
            _rectTransform = null;
        }

        public void Dispose()
        {
        }

        public void Update(int millisecond)
        {
        }

        private void SetActive(bool enabled)
        {
            if (_rectTransform != null)
            {
                _rectTransform.gameObject.SetActive(enabled);
            }
        }

        private void SortOrderView(INode parent)
        {
            var siblingIndex = 0;
            for (var i = 0; i < parent.ChildCount; i++)
            {
                siblingIndex = i;

                // if (SortingOrder >= childPage.View.SortingOrder)
                //     continue;
            }

            using (var profiler = new AutoReleaseProfiler("SetSibling"))
            {
                if (_rectTransform.GetSiblingIndex() != siblingIndex)
                    _rectTransform.SetSiblingIndex(siblingIndex);
            }
            // using (var profiler = new AutoReleaseProfiler("sortingOrder"))
            // {
            //     int sortingOrder = 0;
            //     if (parent is UINode parentUINode)
            //     {
            //         var parentPanel = parentUINode.View;
            //         sortingOrder = parentPanel.SortingOrder;
            //     }

            //     int newOrder = sortingOrder + siblingIndex;
            //     if (_canvas.sortingOrder != newOrder)
            //     {
            //         _canvas.sortingOrder = sortingOrder + siblingIndex;
            //     }
            // }

        }

        private void SetTransitionProgress(float progress)
        {
            TransitionAnimationProgress = progress;
            TransitionAnimationProgressChanged?.Invoke(progress);
        }

        private RectTransform GetParentTrans(INode parent)
        {
            if (parent is UINode uiNode)
            {
                return uiNode.View.RectTransform;
            }
            else if (parent is SceneNode sceneNode)
            {
                return _gameContainer.GetInstance(
                    ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME).transform as RectTransform;
            }

            return null;
        }

        private async PromiseTask TransitionRoutine(bool push, bool enter, IUINode partnerPage, UnsafeCancellationToken cancellation)
        {
            UITransitionAnimation animation = _uiPanel.GetAnimation(push, enter, partnerPage?.Key);
            if (animation == null)
            {
                var transitionBehaviour = await _animationContainer.GetDefaultPageTransition(push, enter, cancellation);
                await PlayTransition(partnerPage, transitionBehaviour, cancellation);
                return;
            }

            if (_transitionDict.ContainsKey(animation))
            {
                await PlayTransition(partnerPage, _transitionDict[animation], cancellation);
            }
            else
            {
                var transitionBehaviour = await _animationContainer.GetAnimationBehaviour(animation, cancellation);
                _transitionDict.Add(animation, transitionBehaviour);
                await PlayTransition(partnerPage, transitionBehaviour, cancellation);
            }
        }

        private async PromiseTask PlayTransition(IUINode partnerPage, IUITransitionAnimationBehaviour transitionBehaviour, UnsafeCancellationToken cancellation)
        {
            if (transitionBehaviour.Duration > 0.0f)
            {
                transitionBehaviour.SetPartner(partnerPage?.View.RectTransform);
                transitionBehaviour.Setup(_rectTransform);
                // PLAN replace with promise task
                // 🤔🤔🤔why we use scene(Game) timer?
                // since remove node will be happened in transition
                var promise = _sceneTimer.Schedule(transitionBehaviour.Duration, (timeData) => transitionBehaviour.SetTime(timeData.elapsedTime), cancellation)
                    .Progress(TransitionProgressReporter);

                // promise.Catch(ex => Console.Info("ExitAnim {ex}", ex));
                await promise.Task;
            }
        }


    }
}
