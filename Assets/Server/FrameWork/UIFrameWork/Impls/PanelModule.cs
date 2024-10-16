using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.AssetContainers;
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
        protected override long _loadTimeOutTime
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

        protected override string GetNodeGUID(IAssetKey assetKey)
        {
            if (_treeNodes.TryGetValue(assetKey, out var node) && node is UINode uINode)
            {
                return uINode.PanelID;
            }
            else
            {
                return base.GetNodeGUID(assetKey);
            }
        }

        #region Load
        protected override void DispatchSwitch(IAssetKey curUI, IAssetKey lastUI)
        {
            var e = new SwitchUIEvent();
            e.LastUI = lastUI;
            e.CurUI = curUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetUI, string guid)
        {
            var e = new AddUIBeginEvent();
            e.TargetUI = targetUI;
            e.ParentUI = _parentNode.Key;
            e.GUID = guid;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetUI)
        {
            var e = new AddUIEndEvent();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddFail(IAssetKey targetUI, string guid, bool isUnload = true)
        {
            var e = new AddUIFailEvent();
            e.TargetUI = targetUI;
            e.ParentUI = _parentNode.Key;
            e.IsUnload = isUnload;
            e.GUID = guid;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetUI)
        {
            var e = new RemoveUIBeginEvent();
            e.TargetUI = targetUI;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetUI, bool isUnload)
        {
            var e = new RemoveUIEndEvent();
            e.TargetUI = targetUI;
            e.ParentUI = _parentNode.Key;
            e.IsUnload = isUnload;
            _eventBus.Dispatch(e);
        }

        #endregion

        protected override async PromiseTask<INode> CreateNode(IAssetKey key)
        {
            await PromiseTask.CompletedTask;
            var uINode = UICreator.CreatePanelNode((UIKey)key);
            uINode.AssignContext(new PanelContext());
            return uINode;
        }
    }
}
