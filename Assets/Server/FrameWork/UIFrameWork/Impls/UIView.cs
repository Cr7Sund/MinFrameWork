using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls;
using UnityEngine;
using Cr7Sund.Server.Utils;
using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Impl;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEditor;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIView : IUIView
    {
        public const string UI_ROOT_NAME = "UIROOT";

        [Inject] private IPromiseTimer _timer;
        [Inject] private UITransitionAnimationContainer _animationContainer;
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

        public void Start(Object asset, INode parent)
        {
            //Instantiate
            AssertUtil.IsNull(_rectTransform, UIExceptionType.instantiate_UI_repeat);

            _parentTransform = GetParentTrans(parent);

            var go = GameObject.Instantiate(asset, _parentTransform) as GameObject;
            _uiPanel = go.GetComponent<UIPanel>();

            var gameObject = _uiPanel.gameObject;

            _rectTransform = (RectTransform)gameObject.transform;
            _canvasGroup = _uiPanel.GetUIComponent<CanvasGroup>(nameof(CanvasGroup));
            _canvas = _uiPanel.GetUIComponent<Canvas>(nameof(Canvas));
            _identifier = gameObject.name.Replace("(Clone)", string.Empty);
        }

        public void Enable(INode parent)
        {
            SortOrderView(parent);
        }

        public void BeforeEnter()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // Set order of rendering.

            SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);
            _canvasGroup.alpha = 0.0f;
        }

        public virtual IPromise EnterRoutine(bool push, IUINode partnerPage, bool playAnimation)
        {
            if (!Application.isPlaying)
            {
                return Promise.Resolved();
            }

            _canvasGroup.alpha = 1.0f;

            if (playAnimation)
            {
                return TransitionRoutine(push, true, partnerPage);
            }

            return Promise.Resolved();
        }

        public void AfterEnter()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(1.0f);
        }

        public void BeforeExit()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1.0f;
        }

        public IPromise ExitRoutine(bool push, IUINode partnerPage, bool playAnimation)
        {
            if (!Application.isPlaying)
            {
                return Promise.Resolved();
            }

            if (playAnimation)
            {
                return TransitionRoutine(push, false, partnerPage);
            }

            return Promise.Resolved();
        }

        public void AfterExit()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _canvasGroup.alpha = 0.0f;
            SetTransitionProgress(1.0f);
            SetActive(false);
        }
        
        public void Stop()
        {
            foreach (var item in _transitionDict)
            {
                _animationContainer.UnloadAnimation(item.Key);
            }

            _transitionDict.Clear();

            if (_rectTransform)
            {
                GameObject.Destroy(_rectTransform);
            }
            _rectTransform = null;
        }

        public void Dispose()
        {
            Stop();
        }

        private void SetActive(bool enabled)
        {
            RectTransform.gameObject.SetActive(enabled);
        }

        private void SortOrderView(INode parent)
        {
            var siblingIndex = 0;
            for (var i = 0; i < parent.ChildCount; i++)
            {
                var parentNode = parent as Node;
                var childPage = parentNode[i] as UINode;
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

        public RectTransform GetParentTrans(INode parent)
        {
            if (parent is UINode uiNode)
            {
                return uiNode.View.RectTransform;
            }
            else if (parent is SceneNode sceneNode)
            {
                return sceneNode.SceneContainer.
                CreateInstanceWithComponent<RectTransform>(UI_ROOT_NAME);
            }

            return null;
        }

        private IPromise TransitionRoutine(bool push, bool enter, IUINode partnerPage)
        {
            UITransitionAnimation animation = _uiPanel.GetAnimation(push, enter, partnerPage?.Key);
            if (animation == null)
            {
                return _animationContainer.GetDefaultPageTransition(push, enter)
                    .Then(transitionBehaviour =>
                    {
                        _transitionDict.Add(animation, transitionBehaviour);
                        return PlayTransition(partnerPage, transitionBehaviour);
                    });
            }

            if (_transitionDict.ContainsKey(animation))
            {
                return PlayTransition(partnerPage, _transitionDict[animation]);
            }
            else
            {
                var animBehaviourPromise = _animationContainer.GetAnimationBehaviour(animation);
                return animBehaviourPromise.Then(transitionBehaviour =>
                {
                    _transitionDict.Add(animation, transitionBehaviour);
                    return PlayTransition(partnerPage, transitionBehaviour);
                });
            }
        }

        private IPromise PlayTransition(IUINode partnerPage, IUITransitionAnimationBehaviour transitionBehaviour)
        {
            if (transitionBehaviour.Duration > 0.0f)
            {
                transitionBehaviour.SetPartner(partnerPage?.View.RectTransform);
                transitionBehaviour.Setup(_rectTransform);
                return _timer.WaitFor(transitionBehaviour.Duration, transitionBehaviour.SetTime)
                             .Progress(TransitionProgressReporter);
            }
            else
            {
                return Promise.Resolved();
            }
        }


    }
}