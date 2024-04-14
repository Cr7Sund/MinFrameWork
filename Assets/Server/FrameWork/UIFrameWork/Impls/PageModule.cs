using System;
using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PageModule : LoadModule, IPageModule
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
        public async PromiseTask BackPage()
        {
            await BackPage(1);
        }

        public async PromiseTask BackPage(int popCount)
        {
            AssertUtil.Greater(_pageContainers.Count, 1, UIExceptionType.NO_LEFT_BACK);
            AssertUtil.Greater(popCount, 0, UIExceptionType.INVALID_POP_COUNT);
            AssertUtil.Greater(_pageContainers.Count, popCount, UIExceptionType.INVALID_BACK);

            int count = 0;
            var curPage = _pageContainers.Peek();
            while (_pageContainers.Count > 0)
            {
                var top = _pageContainers.Pop();

                if (count >= popCount)
                {
                    await PopPage(top.Key, curPage.Key);
                    break;
                }
                count++;
            }

        }

        public async PromiseTask BackPage(IAssetKey popKey)
        {
            AssertUtil.Greater(OperateNum, 1, UIExceptionType.NO_LEFT_BACK);
            ContainsPopPage(popKey);

            var curPage = _pageContainers.Peek();
            while (OperateNum > 0)
            {
                var top = _pageContainers.Pop();
                if (top.Key == popKey)
                {
                    await PopPage(popKey, curPage?.Key);
                    break;
                }
            }
        }

        public async PromiseTask PushPage(IAssetKey uiKey)
        {
            AssertUtil.IsInstanceOf<SceneNode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PAGE_PARENT);

            IAssetKey exitKey = GetLastView();
            var enterKey = uiKey as UIKey;
            var exitPage = GetViewByKey<UINode>(exitKey);
            var enterPage = GetViewByKey<UINode>(enterKey);
            if (IsInTransition)
            {
                throw new MyException(UIExceptionType.OPEN_IN_TRANSITION);
            }

            enterKey.exitPageKey = exitKey;
            enterKey.IsPush = true;
            enterKey.ShowAfter = exitPage != null && enterKey.HideFirst &&
                                 enterPage == null;

            if (enterKey.ShowAfter)
            {
                var hideFirstPromise = HideFirstSequence(exitKey);
                var addPromise = AddNode(enterKey);
                await PromiseTask.WhenAll(hideFirstPromise, addPromise);
                await OnShowAfter(GetViewByKey<UINode>(enterKey), exitPage);
            }
            else
            {
                await AddNode(enterKey);
            }
        }

        public async PromiseTask PreLoadUI(UIKey key)
        {
            await PreLoadNode(key);
        }

        protected override INode CreateNode(IAssetKey key)
        {
            var uINode = UICreator.Create((UIKey)key);
            uINode.AssignContext(new PageContext());
            return uINode;
        }

        protected override async PromiseTask OnAdded(IAssetKey enterKey)
        {
            var enterPageKey = enterKey as UIKey;
            var exitPageKey = enterPageKey.exitPageKey;
            var exitPage = GetViewByKey<UINode>(exitPageKey);
            var enterPage = GetViewByKey<UINode>(enterKey);

            if (!enterPageKey.ShowAfter)
            {
                AddIntoStack(enterPage);

                // 1. 
                // OpenSequence including controller's lifetime
                // we can switch page on controllers' transition( the animation it's waiting too long)
                // so we need to ensure the internal state that it's done. such as ui stack
                // and also the controller lifecycle will been called after the transition  

                // 2. something wrong with OpenSequence ?
                // load already done internal, and the controller have not been called
                // unload can be called too, and also the controller lifeCycle
                // Pay attention to the transition lifeCycle since the afterAnimation will not be called
                // if there is a exception in transition (this can be handled rejected if you want ) 
                if (exitPage != null)
                {
                    try
                    {
                        await OpenSequence(enterPage, exitPage);
                    }
                    finally
                    {
                        await RemovePage(enterPage, exitPage);
                    }

                }
                else
                {
                    await OpenSequence(enterPage, exitPage);
                }
            }

        }

        private void AddIntoStack(UINode enterPage)
        {
            // var enterKey = enterPage.Key as UIKey;
            if (OperateNum > 0)
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

        private async PromiseTask RemovePage(INode node, UINode exitPage)
        {
            var exitKey = exitPage.Key as UIKey;

            if (exitKey.Stack)
            {
                await RemoveNode(exitPage.Key);
            }
            else
            {
                await UnloadNode(exitPage.Key);
            }
        }

        private async PromiseTask OnShowAfter(UINode enterPage, UINode exitPage)
        {
            AddIntoStack(enterPage);
            await RemovePage(enterPage, exitPage);
            await ShowAfterSequence(enterPage, exitPage);
        }

        private async PromiseTask PopPage(IAssetKey uiKey, IAssetKey exitKey)
        {
            var enterKey = uiKey as UIKey;
            enterKey.exitPageKey = exitKey;
            enterKey.IsPush = false;

            await AddNode(enterKey);
        }

        private IAssetKey GetLastView()
        {
            return OperateNum > 0 ? _pageContainers.Peek().Key : null;
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

        private async PromiseTask OpenSequence(IUINode enterPage, IUINode exitPage)
        {
            var enterUIKey = enterPage.Key as UIKey;
            bool isPush = enterUIKey.IsPush;

            await enterPage.BeforeEnter(isPush, exitPage);
            if (exitPage != null)
            {
                var exitUIKey = exitPage.Key as UIKey;
                await exitPage.BeforeExit(isPush, enterPage);
                await exitPage.Exit(isPush, enterPage, exitUIKey.PlayAnimation);
                await exitPage.AfterExit(isPush, enterPage);
            }
            await enterPage.Enter(isPush, exitPage, enterUIKey.PlayAnimation);
            await enterPage.AfterEnter(push: isPush, exitPage);
        }

        private async PromiseTask HideFirstSequence(IAssetKey exitKey)
        {
            if (exitKey == null)
            {
                return;
            }

            IUINode enterPage = null;
            // IUINode enterPage=  UINode.CreateBlackScreen();
            var exitUIKey = exitKey as UIKey;
            var exitPage = GetViewByKey<UINode>(exitUIKey);
            bool isPush = exitUIKey.IsPush;

            await exitPage.BeforeExit(isPush, enterPage);
            await exitPage.Exit(isPush, enterPage, exitUIKey.PlayAnimation);
        }

        private async PromiseTask ShowAfterSequence(IUINode enterPage, IUINode exitPage)
        {
            var enterUIKey = enterPage.Key as UIKey;
            bool isPush = enterUIKey.IsPush;

            await enterPage.BeforeEnter(isPush, exitPage);
            await enterPage.Enter(isPush, exitPage, enterUIKey.PlayAnimation);
            if (exitPage != null)
            {
                await exitPage.AfterExit(isPush, enterPage);
            }
            await enterPage.AfterEnter(isPush, exitPage);
        }

        #endregion

        #region Event
        protected override void DispatchSwitch(IAssetKey curUI, IAssetKey lastUI)
        {
            var e = _eventBus.CreateEvent<SwitchUIEvent>();
            e.LastUI = lastUI;
            e.CurUI = curUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetUI)
        {
            var e = _eventBus.CreateEvent<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetUI)
        {
            var e = _eventBus.CreateEvent<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetUI)
        {
            var e = _eventBus.CreateEvent<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetUI)
        {
            var e = _eventBus.CreateEvent<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            AssertUtil.LessOrEqual(OperateNum, 0);
            _pageContainers = null;
        }

        public async PromiseTask CloseAll()
        {
            await UnloadAllNodes();
            _pageContainers.Clear();
        }
    }
}
