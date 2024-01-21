using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;
using UnityEngine.Analytics;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIModule : LoadModule, IUIModule
    {
        public INode FocusUI { get; set; }
        public byte OperateNum => throw new System.NotImplementedException();

        //Stack
        private List<IUIView> _pageContainers;

        public void Back()
        {
            throw new System.NotImplementedException();
        }

        public void Back(int popCount = 1)
        {
            throw new System.NotImplementedException();
        }

        public void Back(IAssetKey popKey)
        {
            throw new System.NotImplementedException();
        }

        public void Close(IAssetKey uiKey)
        {
            throw new System.NotImplementedException();
        }

        public void Freeze()
        {
            throw new System.NotImplementedException();
        }

        public IUIView GetLastView()
        {
            throw new System.NotImplementedException();
        }

        public bool IsVisible(IAssetKey uiKey)
        {
            throw new System.NotImplementedException();
        }

        #region PageContainers

        // _pageContainer.Bind
        // _pageContainer.OpenPanel();
        
        // only call from Scene
        // or switch Pages
        public IPromise OpenPanel(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true, bool hideFirst = false)
        {
            IUISequence sequence = GetUISequence(hideFirst);
            var viewContent = ViewContent.AssignViewContent(uiKey, intent, playAnimation, stack, loadAsync);

            Freeze();
            return sequence.Open(viewContent)
                        .Then(() =>
                            {
                                var exitPage = GetLastView() as UINode;
                                _pageContainers.RemoveAt(_pageContainers.Count - 1);
                                return RemoveNode(exitPage.Key);
                            })
                        .Then(_ =>
                            {
                                if (_treeNodes.TryGetValue(uiKey, out var node))
                                {
                                    var uiNode = node as UINode;
                                    _pageContainers.Add(uiNode);
                                }

                                UnFreeze();
                            });
        }

        // Only Call from Page
        public IPromise AddPanels(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true, bool hideFirst = false)
        {
            IUISequence sequence = GetUISequence(hideFirst);
            var viewContent = ViewContent.AssignViewContent(uiKey, intent, playAnimation, stack, loadAsync);

            Freeze();
            return sequence.Open(viewContent)
                         .Then(() =>
                            {
                                if (_treeNodes.TryGetValue(uiKey, out var node))
                                {
                                    var uiNode = node as UINode;
                                    _pageContainers.Add(uiNode);
                                }

                                UnFreeze();
                            });
        }
        public IPromise SwitchPanels(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true, bool hideFirst = false)
        {
            IUISequence sequence = GetUISequence(hideFirst);
            var viewContent = ViewContent.AssignViewContent(uiKey, intent, playAnimation, stack, loadAsync);

            Freeze();
            return sequence.Open(viewContent)
                            .Then(() =>
                            {
                                //foreach
                                var exitPage = GetLastView() as UINode;
                                _pageContainers.RemoveAt(_pageContainers.Count - 1);
                                return RemoveNode(exitPage.Key);
                            })
                            .Then(OnSwitchNode)
                            .Then(_ =>
                            {
                                if (_treeNodes.TryGetValue(uiKey, out var node))
                                {
                                    var uiNode = node as UINode;
                                    _pageContainers.Add(uiNode);
                                }

                                UnFreeze();
                            })
                            ;
        }
        private IUISequence GetUISequence(bool hideFirst)
        {
            if (hideFirst)
            {
                var sequence = new SyncUISequence();
                sequence.Module = this;
                return sequence;
            }
            else
            {
                var sequence = new SyncUISequence();
                sequence.Module = this;
                return sequence;
            }
        }

        #endregion

        public void OpenModal(IAssetKey uiKey)
        {
            throw new System.NotImplementedException();
        }

        public void OpenTab(IAssetKey uiKey)
        {
            throw new System.NotImplementedException();
        }

        #region  View Transition Sequence
        public bool IsInTransition;




        public void UnFreeze()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Load

        public IPromise<INode> PreLoadUI(UIKey key)
        {
            return PreLoadNode(key);
        }
        public IPromise<INode> AddUI(UIKey key)
        {
            return AddNode(key).Then((node) =>
            {
                return node;
            });
        }
        public IPromise<INode> RemoveUI(UIKey key)
        {
            return RemoveNode(key);
        }
        public IPromise<INode> UnloadUI(UIKey key)
        {
            return UnloadNode(key);
        }
        public IPromise<INode> SwitchUI(UIKey key)
        {
            return SwitchNode(key);
        }

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