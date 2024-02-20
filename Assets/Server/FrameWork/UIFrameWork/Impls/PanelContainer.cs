using System;
using System.Collections.Generic;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Touch.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PanelContainer : LoadModule, IUIContainer
    {
        [Inject]
        private IUINode _parentUINode;
        protected override INode _parentNode
        {
            get
            {
                return _parentUINode;
            }
        }
        public int OperateNum
        {
            get
            {
                return _parentNode.ChildCount;
            }
        }

        // Only Call from Page
        public IPromise<INode> OpenPanel(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true)
        {
            AssertUtil.IsInstanceOf<UINode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PANEL_PARENT);

            return AddNode(uiKey);
        }

        public IPromise<INode> OpenPanelAndCloseOthers(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true)
        {
            AssertUtil.IsInstanceOf<UINode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PANEL_PARENT);

            return SwitchNode(uiKey);
        }

        protected override IPromise<INode> OnAdded(INode node)
        {
            var enterPage = node as UINode;
            var enterKey = enterPage.Key as UIKey;

            return OpenSequence(enterPage)
                    .Then(() => Promise<INode>.Resolved(node));
        }

        private IPromise CloseSequence(IUINode exitPage)
        {
            if (exitPage != null)
            {
                return Promise.Resolved();
            }

            IUINode enterPage = null;
            // IUINode enterPage=  UINode.CreateBlackScreen();
            var exitPageUIKey = exitPage.Key as UIKey;

            var handlers = new List<Func<IPromise>>();

            Func<IPromise> beforeExitHandler = () =>
                    exitPage.BeforeExit(true, enterPage);
            Func<IPromise> exitHandler = () =>
                    exitPage.Exit(true, enterPage, exitPageUIKey.PlayAnimation);
            Func<IPromise> afterExitHandler = () =>
                    exitPage.AfterExit(true, enterPage);

            handlers.Add(beforeExitHandler);
            handlers.Add(exitHandler);
            handlers.Add(afterExitHandler);
            return Promise.Sequence(handlers);
        }


        private IPromise OpenSequence(IUINode enterPage)
        {
            var handlers = new List<Func<IPromise>>();
            IUINode exitPage = null;
            var enterPageUIKey = enterPage.Key as UIKey;

            Func<IPromise> beforeEnterHandler = () => enterPage.BeforeEnter(true, exitPage);
            Func<IPromise> enterHandler = () => enterPage.Enter(true, exitPage, enterPageUIKey.PlayAnimation);
            Func<IPromise> afterEnterHandler = () =>
                enterPage.AfterEnter(true, exitPage);

            handlers.Add(beforeEnterHandler);
            handlers.Add(enterHandler);
            handlers.Add(afterEnterHandler);

            return Promise.Sequence(handlers);
        }



        #region Load
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
        protected override INode CreateNode(IAssetKey key)
        {
            return UICreator.Create((UIKey)key);
        }
        #endregion

    }
}
