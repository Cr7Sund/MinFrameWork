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
        protected override int _loadTimeOutTime
        {
            get
            {
                return ServerBindDefine.UITimeOutTime;
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

        public async PromiseTask CloseAll()
        {
            await UnloadAllNodes();
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
        protected override async PromiseTask<INode> CreateNode(IAssetKey key)
        {
            await PromiseTask.CompletedTask;
            var uINode = UICreator.CreatePanelNode((UIKey)key);
            uINode.AssignContext(new PanelContext());
            return uINode as INode;
        }
        #endregion


    }
}
