using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PanelModule : LoadModule, IPanelModule
    {
        [Inject] private IUINode _parentUINode;
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
        public async PromiseTask OpenPanel(IAssetKey uiKey)
        {
            AssertUtil.IsInstanceOf<UINode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PANEL_PARENT);

            await AddNode(uiKey);
        }

        public async PromiseTask OpenPanelAndCloseOthers(IAssetKey uiKey)
        {
            AssertUtil.IsInstanceOf<UINode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PANEL_PARENT);

            await SwitchNode(uiKey);
        }

        protected override async PromiseTask OnAdded(IAssetKey assetKey)
        {
            await OpenSequence(assetKey);
        }

        protected override async PromiseTask OnRemoved(IAssetKey key)
        {
            await CloseSequence(key);
        }

        private async PromiseTask CloseSequence(IAssetKey exitKey)
        {
            var exitPage = GetViewByKey<UINode>(exitKey);
            if (exitPage == null)
            {
                return;
            }

            IUINode enterPage = null;
            // IUINode enterPage=  UINode.CreateBlackScreen();
            var exitPageUIKey = exitPage.Key as UIKey;

            await exitPage.BeforeExit(true, enterPage);
            await exitPage.Exit(true, enterPage, exitPageUIKey.PlayAnimation);
            await exitPage.AfterExit(true, enterPage);
        }

        private async PromiseTask OpenSequence(IAssetKey enterKey)
        {
            var enterPageUIKey = enterKey as UIKey;
            var exitPage = GetViewByKey<UINode>(enterPageUIKey.exitPageKey);
            var enterPage = GetViewByKey<UINode>(enterKey);

            await enterPage.BeforeEnter(true, exitPage);
            await enterPage.Enter(true, exitPage, enterPageUIKey.PlayAnimation);
            await enterPage.AfterEnter(true, exitPage);
        }


        #region Load
        protected override void DispatchSwitch(IAssetKey curUI, IAssetKey lastUI)
        {
            var e = _eventBus.CreateEvent<SwitchUIEvent>();
            e.LastUI = lastUI;
            e.CurUI = curUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetUI)
        {
            var e =  _eventBus.CreateEvent<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetUI)
        {
            var e =  _eventBus.CreateEvent<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetUI)
        {
            var e =  _eventBus.CreateEvent<AddUIBeginEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetUI)
        {
            var e =  _eventBus.CreateEvent<AddUIEndEvent>();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override INode CreateNode(IAssetKey key)
        {
            var uINode = UICreator.Create((UIKey)key);
            uINode.AssignContext(new PanelContext());
            return uINode;
        }
        #endregion

        public async PromiseTask CloseAll()
        {
            await UnloadAllNodes();
        }
    }
}
