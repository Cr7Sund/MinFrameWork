using System;
using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PageContainer : LoadModule, IUIContainer
    {
        [Inject]
        private ISceneNode _sceneNode;
        private Stack<IUINode> _pageContainers = new Stack<IUINode>(16);

        protected override INode _parentNode
        {
            get
            {
                return _sceneNode;
            }
        }
        public int OperateNum
        {
            get
            {
                return _pageContainers.Count;
            }
        }


        #region  Load
        public void BackPage()
        {
            BackPage(1);
        }

        public void BackPage(int popCount)
        {
            AssertUtil.Greater(_pageContainers.Count, 1, UIExceptionType.NO_LEFT_BACK);
            AssertUtil.Greater(_pageContainers.Count, popCount, UIExceptionType.INVALID_BACK);

            int count = 0;
            var curPage = _pageContainers.Peek();
            while (_pageContainers.Count > 0)
            {
                var top = _pageContainers.Pop() as UINode;

                if (count >= popCount)
                {
                    PopPage(top.Key, curPage.Key);
                    break;
                }
                count++;
            }

        }

        public void BackPage(IAssetKey popKey)
        {
            AssertUtil.Greater(_pageContainers.Count, 1, UIExceptionType.NO_LEFT_BACK);
            ContainsPopPage(popKey);

            var curPage = _pageContainers.Peek();
            while (_pageContainers.Count > 0)
            {
                var top = _pageContainers.Pop() as UINode;
                if (top.Key == popKey)
                {
                    PopPage(popKey, curPage?.Key);
                    break;
                }
            }

        }

        public IPromise<INode> PushPage(IAssetKey uiKey)
        {
            AssertUtil.IsInstanceOf<SceneNode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PAGE_PARENT);

            IAssetKey exitKey = GetLastView();
            var enterKey = uiKey as UIKey;
            var exitPage = GetViewByKey(exitKey);
            var enterPage = GetViewByKey(enterKey);

            enterKey.exitPage = exitKey;
            enterKey.IsPush = true;
            enterKey.ShowAfter = exitPage != null && enterKey.HideFirst &&
                                 enterPage == null;

            if (enterKey.ShowAfter)
            {
                if (!exitPage.IsTransitioning)
                {
                    var hideFirstPromise = HideFirstSequence(exitPage)
                                .Then(() => Promise<INode>.Resolved(exitPage));
                    var addPromise = AddNode(enterKey);
                    return Promise<INode>.All(hideFirstPromise, addPromise)
                            .Then((nodes) => OnShowAfter(nodes, exitPage));
                }
                else
                {
                    return Promise<INode>.Rejected(new MyException(UIExceptionType.HIDE_IN_TRANSITION));
                }
            }
            else
            {
                return AddNode(enterKey);
            }
        }

        public IPromise<INode> PreLoadUI(UIKey key)
        {
            return PreLoadNode(key);
        }

        protected override INode CreateNode(IAssetKey key)
        {
            var uINode = UICreator.Create((UIKey)key);
            uINode.AssignContext(new UIContext());
            return uINode;
        }
      
        protected override IPromise<INode> OnAdded(INode node)
        {
            var enterPage = node as UINode;
            var enterKey = enterPage.Key as UIKey;
            var exitPageKey = enterKey.exitPage;
            var exitPage = GetViewByKey(exitPageKey);

            if (!enterKey.ShowAfter)
            {
                return OpenSequence(enterPage, exitPage)
                                   .Then(() =>
                                       {
                                           AddIntoStack(enterPage);

                                           if (exitPage != null)
                                           {
                                               return RemovePage(node, exitPage);
                                           }
                                           else
                                           {
                                               return Promise<INode>.Resolved(node);
                                           }
                                       });
            }
            else
            {
                return Promise<INode>.Resolved(node);
            }

        }

        private void AddIntoStack(UINode enterPage)
        {
            // var enterKey = enterPage.Key as UIKey;
            if (_pageContainers.Count > 0)
            {
                var topNode = _pageContainers.Peek() as UINode;
                var topKey = topNode.Key as UIKey;
                if (!topKey.Stack)
                {
                    _pageContainers.Pop();
                }
            }
            _pageContainers.Push(enterPage);
        }

        private IPromise<INode> RemovePage(INode node, UINode exitPage)
        {
            var exitKey = exitPage.Key as UIKey;

            return exitKey.Stack ? RemoveNode(exitPage.Key) : UnloadNode(exitPage.Key)
                    .Then(_ => node);
        }

        private IPromise<INode> OnShowAfter(IEnumerable<INode> nodes, UINode exitPage)
        {
            INode enterNode = null;
            foreach (var item in nodes)
            {
                if (item != exitPage)
                {
                    enterNode = item;
                    break;
                }
            }

            var enterPage = enterNode as UINode;
            var enterKey = enterNode.Key as UIKey;
            var exitPageKey = enterKey.exitPage;

            return ShowAfterSequence(enterPage, exitPage)
                    .Then(() =>
                    {
                        AddIntoStack(enterPage);

                        if (exitPage != null)
                        {
                            return RemovePage(enterNode, exitPage);
                        }
                        else
                        {
                            return Promise<INode>.Resolved(enterNode);
                        }
                    });
        }
        private IPromise<INode> PopPage(IAssetKey uiKey, IAssetKey exitKey)
        {
            var enterKey = uiKey as UIKey;
            enterKey.exitPage = exitKey;
            enterKey.IsPush = false;

            return AddNode(enterKey);
        }

        private IAssetKey GetLastView()
        {
            return _pageContainers.Count > 0 ? _pageContainers.Peek().Key : null;
        }

        private UINode GetViewByKey(IAssetKey exitKey)
        {
            UINode exitPage = null;

            if (exitKey != null)
            {
                if (_treeNodes.TryGetValue(exitKey, out var exitNode))
                {
                    exitPage = exitNode as UINode;
                }
            }

            return exitPage;
        }

        private void ContainsPopPage(IAssetKey popKey)
        {
            bool existBackUI = false;
            foreach (var item in _pageContainers)
            {
                var node = item as UINode;
                if (node.Key == popKey)
                {
                    existBackUI = true;
                    break;
                }
            }
            if (!existBackUI)
            {
                throw new MyException($"try to back an invalid ui : {popKey} ");
            }
        }


        #endregion


        #region View Transition Sequence

        private IPromise OpenSequence(IUINode enterPage, IUINode exitPage)
        {
            var handlers = new List<Func<IPromise>>();
            var enterUIKey = enterPage.Key as UIKey;
            bool isPush = enterUIKey.IsPush;

            Func<IPromise> beforeEnterHandler = () => enterPage.BeforeEnter(isPush, exitPage);
            Func<IPromise> beforeExitHandler = null;
            Func<IPromise> exitHandler = null;
            Func<IPromise> afterExitHandler = null;
            if (exitPage != null)
            {
                var exitUIKey = exitPage.Key as UIKey;
                beforeExitHandler = () =>
                     exitPage.BeforeExit(isPush, enterPage);
                exitHandler = () =>
                     exitPage.Exit(isPush, enterPage, exitUIKey.PlayAnimation);
                afterExitHandler = () =>
                     exitPage.AfterExit(isPush, enterPage);
            }
            Func<IPromise> enterHandler = () =>
                   enterPage.Enter(isPush, exitPage, enterUIKey.PlayAnimation);
            Func<IPromise> afterEnterHandler = () =>
                   enterPage.AfterEnter(push: isPush, exitPage);


            handlers.Add(beforeEnterHandler);
            if (exitPage != null)
            {
                handlers.Add(beforeExitHandler);
                handlers.Add(exitHandler);
            }
            handlers.Add(enterHandler);
            if (exitPage != null)
            {
                handlers.Add(afterExitHandler);
            }
            handlers.Add(afterEnterHandler);

            return Promise.Sequence(handlers);
        }

        private IPromise HideFirstSequence(IUINode exitPage)
        {
            if (exitPage == null)
            {
                return Promise.Resolved();
            }

            IUINode enterPage = null;
            // IUINode enterPage=  UINode.CreateBlackScreen();
            var exitUIKey = exitPage.Key as UIKey;
            bool isPush = exitUIKey.IsPush;


            var handlers = new List<Func<IPromise>>();

            Func<IPromise> beforeExitHandler = () =>
                exitPage.BeforeExit(isPush, enterPage);
            Func<IPromise> exitHandler = () =>
                exitPage.Exit(isPush, enterPage, exitUIKey.PlayAnimation);

            handlers.Add(beforeExitHandler);
            handlers.Add(exitHandler);
            return Promise.Sequence(handlers);
        }

        private IPromise ShowAfterSequence(IUINode enterPage, IUINode exitPage)
        {
            var handlers = new List<Func<IPromise>>();
            var enterUIKey = enterPage.Key as UIKey;
            bool isPush = enterUIKey.IsPush;

            Func<IPromise> beforeEnterHandler = () => enterPage.BeforeEnter(isPush, exitPage);
            Func<IPromise> enterHandler = () => enterPage.Enter(isPush, exitPage, enterUIKey.PlayAnimation);
            Func<IPromise> afterExitHandler = null;
            if (exitPage != null)
            {
                afterExitHandler = () =>
                    exitPage.AfterExit(isPush, enterPage);
            }
            Func<IPromise> afterEnterHandler = () =>
                             enterPage.AfterEnter(isPush, exitPage);

            handlers.Add(beforeEnterHandler);
            handlers.Add(enterHandler);
            if (exitPage != null)
            {
                handlers.Add(afterExitHandler);
            }
            handlers.Add(afterEnterHandler);

            return Promise.Sequence(handlers);
        }

        #endregion


        #region Event
        protected override void DispatchSwitch(IAssetKey curUI, IAssetKey lastUI)
        {
            var e = _poolBinder.AutoCreate<SwitchUIEvent>();
            e.LastUI = lastUI;
            e.CurUI = curUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetUI)
        {
            var e = _poolBinder.AutoCreate<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetUI)
        {
            var e = _poolBinder.AutoCreate<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetUI)
        {
            var e = _poolBinder.AutoCreate<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetUI)
        {
            var e = _poolBinder.AutoCreate<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }

        #endregion

    }
}
