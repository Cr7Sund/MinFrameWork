using System.Collections.Generic;
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

        protected override int _loadTimeOutTime
        {
            get
            {
                return ServerBindDefine.UITimeOutTime;
            }
        }

        #region Load
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
            var topPageKey = _pageContainers.Peek().Key;

            while (_pageContainers.Count > 0)
            {
                var top = _pageContainers.Pop();

                if (count >= popCount)
                {
                    await PopPage(top.Key, topPageKey);
                    break;
                }
                count++;
            }
        }

        public async PromiseTask BackPage(IAssetKey popKey)
        {
            AssertUtil.Greater(OperateNum, 1, UIExceptionType.NO_LEFT_BACK);
            ContainsPopPage(popKey);

            var topPageKey = _pageContainers.Peek().Key;

            while (OperateNum > 0)
            {
                var top = _pageContainers.Pop();
                if (top.Key == popKey)
                {
                    await PopPage(top.Key, topPageKey);
                    break;
                }
            }
        }

        public async PromiseTask PushPage(IAssetKey uiKey, bool overwrite = false)
        {
            AssertUtil.IsInstanceOf<SceneNode, UIExceptionType>(_parentNode, UIExceptionType.INVALID_PAGE_PARENT);

            if (IsTransitioning())
            {
                if (overwrite)
                {
                    CancelNode(_focusNode.Key);
                }
                else
                {
                    throw new MyException(UIExceptionType.OPEN_IN_TRANSITION);
                }
            }

            var enterKey = uiKey as UIKey;
            var exitKey = GetLastView() as UIKey;
            if (exitKey == enterKey)
            {
                exitKey = null;
            }
            var exitPage = GetViewByKey<UINode>(exitKey);
            enterKey.exitPage = GetViewByKey<UINode>(exitKey);
            enterKey.IsPush = true;

            bool hideFirst = exitPage != null && enterKey.HideFirst &&
                                 GetViewByKey<UINode>(enterKey) == null;

            if (hideFirst)
            {
                // PLAN : black screen

                var removeTask = RemovePage(exitKey);
                var addTask = AddNode(enterKey);
                await removeTask;
                await addTask;
            }
            else
            {
                await AddNode(enterKey);
                if (exitKey != null)
                {
                    await RemovePage(exitKey);
                }
            }
        }

        public async PromiseTask PreLoadUI(UIKey key)
        {
            await PreLoadNode(key);
        }

        public async PromiseTask CloseAll()
        {
            await UnloadAllNodes();
            _pageContainers.Clear();
        }

        protected override INode CreateNode(IAssetKey key)
        {
            var uINode = UICreator.CreatePageNode((UIKey)key);
            uINode.AssignContext(new PageContext());
            return uINode;
        }

        protected override void OnAdded(IAssetKey enterKey)
        {
            var enterPage = GetViewByKey<UINode>(enterKey);
            AddIntoStack(enterPage);
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

        private async PromiseTask PopPage(IAssetKey destKey, IAssetKey topKey)
        {
            var enterKey = destKey as UIKey;
            var exitKey = topKey as UIKey;
            enterKey.exitPage = GetViewByKey<UINode>(topKey);
            enterKey.IsPush = false;


            await RemovePage(exitKey);
            await AddNode(enterKey);
        }

        private async PromiseTask RemovePage(UIKey exitKey)
        {
            if (exitKey != null)
            {
                if (exitKey.Stack) await RemoveNode(exitKey);
                else await UnloadNode(exitKey);
            }
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


    }
}
