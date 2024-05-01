using System;
using System.Collections.Generic;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Touch.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Server.Impl
{
    public abstract class LoadModule : ILoadModule
    {
        protected INode _focusNode;
        [Inject]
        private IFingerGesture _fingerGesture;
        [Inject]
        protected IEventBus _eventBus;
        [Inject]
        protected IPoolBinder _poolBinder;
        protected Dictionary<IAssetKey, INode> _treeNodes;


        protected abstract INode _parentNode { get; }


        internal LoadModule()
        {
            _treeNodes = new Dictionary<IAssetKey, INode>();
        }


        public async PromiseTask AddNode(IAssetKey assetKey, bool overwrite = false)
        {
            if (!_parentNode.IsStarted)
            {
                throw new MyException(
                    $"NodeModule.AddNode: Do not allow AddNode while ParentNode.Started is false! NodeName: {assetKey}");
            }
            if (assetKey == null)
            {
                return;
            }
            // if (!_parentNode.IsActive)
            // allowed load a bundle of node which mean the parent node maybe not enabled

            Freeze();

            // in case of unhandled case or duplicate operation
            if (_treeNodes.TryGetValue(assetKey, out var assetNode))
            {

                if (assetNode.NodeState == NodeState.Removing)
                {
                    await AddNodeFromRemoving(assetKey);
                    return;
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    await AddNodeFromUnloading(assetKey);
                    return;
                }

                if (assetNode.NodeState == NodeState.Ready)
                {
                    Console.Warn("NodeModule.AddNode: the asset is already on the nodeTree. NodeName: {Key}", assetKey);
                    UnFreeze();
                    return;
                }
                if (assetNode.NodeState == NodeState.Preloading)
                {
                    Console.Warn("NodeModule.LoadNode: the asset is Preloading! NodeName: {Key} ", assetKey);
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    Console.Warn("NodeModule.LoadNode: the asset is adding! NodeName: {Key} ", assetKey);
                }
                if (assetNode.NodeState == NodeState.Preloaded)
                {
                }
                if (assetNode.NodeState == NodeState.Removed)
                {
                }
                await AddNodeFromLoaded(assetKey, overwrite);
                return;
            }

            await AddNodeFromStart(assetKey); //Or Restart
        }

        public async PromiseTask RemoveNode(IAssetKey key, bool overwrite = false)
        {
            await UnloadNodeInternal(key, false, overwrite);
        }

        public async PromiseTask PreLoadNode(IAssetKey assetKey)
        {
            AssertUtil.IsFalse(_treeNodes.ContainsKey(assetKey), FoundationExceptionType.duplicate_preloadNode);

            if (!_parentNode.IsStarted)
            {
                throw new MyException(
                    $"NodeModule.PreLoadNode: Do not allow PreLoadNode while parent node's Started is false! NodeName: {assetKey}");
            }

            if (_treeNodes.TryGetValue(assetKey, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Removed)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset has been removed.(which mean it has already load) NodeName: {Key} ", assetKey);
                    return;
                }
                if (assetNode.NodeState == NodeState.Removing)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is removing. NodeName: {Key} ", assetKey);
                    await PreloadNodeFromRemoving(assetKey);
                    return;
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is unloading. NodeName: {Key} ", assetKey);
                    await PreloadNodeFromRemoving(assetKey);
                    return;
                }

                if (assetNode.NodeState == NodeState.Preloaded)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is preloaded! NodeName: {Key} ", assetKey);
                    return;
                }
                if (assetNode.NodeState == NodeState.Ready)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is already on the nodeTree. NodeName: {Key} ", assetKey);
                    return;
                }

                if (assetNode.NodeState == NodeState.Preloading)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is preloading! NodeName: {Key} ", assetKey);
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    Console.Warn("NodeModule.PreLoadNode: the asset is adding! NodeName: {Key} ", assetKey);
                }
                // we don't care about overwrite when loading
                await PreloadNodeFromLoading(assetKey);
                return;
            }

            await PreloadNodeFromStart(assetKey);
            return;
        }

        public T TestGetViewByKey<T>(IAssetKey assetKey) where T : class, INode
        {
            return GetViewByKey<T>(assetKey);
        }
        
        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_treeNodes.Count, 0);
            _treeNodes = null;
            _focusNode = null;
        }

        public async PromiseTask UnloadNode(IAssetKey key, bool overwrite = false)
        {
            await UnloadNodeInternal(key, true, overwrite);
        }

        internal async PromiseTask UnloadNodeInternal(IAssetKey assetKey, bool unload, bool overwrite)
        {
            if (assetKey == null)
            {
                return;
            }
            Freeze();
            if (!_parentNode.IsStarted)
            {
                UnFreeze();
                throw new MyException(
                    $"NodeModule.RemoveNode: Do not allow unload while parentNode.Started is false! NodeName: {assetKey}");
            }

            if (_treeNodes.TryGetValue(assetKey, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Unloaded)
                {
                    Console.Warn("try to remove an unloaded node: {Key}", assetNode.Key);
                    UnFreeze();
                    return;
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    Console.Warn("try to remove an unloading node: {Key}", assetNode.Key);
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    Console.Warn("NodeModule.RemoveNode: the asset is adding! NodeName: {Key} ", assetKey);
                    assetNode.CancelLoad();
                }

                if (unload == false)
                {
                    await RemoveNodeFromNodeTree(assetKey, overwrite);
                    return;
                }
                else
                {
                    await UnloadNodeFromNodeTree(assetKey, overwrite);
                    return;
                }
            }
        }
        
        internal async PromiseTask SwitchNode(IAssetKey key)
        {
            await AddNode(key);
            await OnSwitchNode(key);
            return;
        }

        protected virtual void Freeze()
        {
            _fingerGesture.Freeze();
        }
        protected virtual void UnFreeze()
        {
            _fingerGesture.UnFreeze();
        }
        protected abstract INode CreateNode(IAssetKey key);

        protected virtual void OnAdded(IAssetKey key)
        {
        }

        protected virtual PromiseTask OnRemoved(IAssetKey key)
        {
            return PromiseTask.CompletedTask;
        }

        protected T GetViewByKey<T>(IAssetKey assetKey) where T : class, INode
        {
            if (assetKey != null)
            {
                if (_treeNodes.TryGetValue(assetKey, out var node))
                {
                    return node as T;
                }
            }

            return default;
        }

        protected async PromiseTask UnloadAllNodes()
        {
            var tmpList = _poolBinder.AutoCreate<List<IAssetKey>>();
            tmpList.Clear();

            foreach (var keyValuePair in _treeNodes)
            {
                var assetNode = keyValuePair.Value;
                tmpList.Add(assetNode.Key);
            }

            for (int i = tmpList.Count - 1; i >= 0; i--)
            {
                await UnloadNode(tmpList[i]);
            }

            tmpList.Clear();
            _poolBinder.Return(tmpList);
        }

        protected async PromiseTask OnSwitchNode(IAssetKey curKey)
        {
            foreach (var keyValuePair in _treeNodes)
            {
                var assetNode = keyValuePair.Value;

                if (assetNode.Key == curKey) continue;
                if (!assetNode.IsActive) continue;

                await RemoveNode(assetNode.Key);
                DispatchSwitch(curKey, assetNode.Key);
            }
        }

        private async PromiseTask RemoveNodeFromNodeTree(IAssetKey removeKey, bool overwrite)
        {
            var removeNode = _treeNodes[removeKey];

            if (removeNode.NodeState == NodeState.Removed)
            {
                UnFreeze();
                return;
            }

            await _parentNode.RemoveChildAsync(removeNode, overwrite);
            await OnRemoveNode(removeKey);
        }

        private async PromiseTask OnRemoveNode(IAssetKey removeKey)
        {
            if (_focusNode != null && _focusNode.Key == removeKey)
                _focusNode = null;

            await OnRemoved(removeKey);
            UnFreeze();
            DispatchRemoveEnd(removeKey);
        }
        private async PromiseTask UnloadNodeFromNodeTree(IAssetKey key, bool overwrite)
        {
            var unloadNode = _treeNodes[key];

            DispatchRemoveBegin(key);

            await _parentNode.UnloadChildAsync(unloadNode, overwrite); // unload will always return resolved
            OnUnloadNode(key);
        }

        private IAssetKey OnUnloadNode(IAssetKey unloadKey)
        {
            UnFreeze();
            if (_focusNode != null && _focusNode.Key == unloadKey)
                _focusNode = null;

            _treeNodes.Remove(unloadKey);
            DispatchRemoveEnd(unloadKey);

            return unloadKey;
        }


        private async PromiseTask PreloadNodeFromStart(IAssetKey key)
        {
            var newNode = CreateNode(key);
            _treeNodes.Add(key, newNode);

            await _parentNode.PreLoadChild(newNode);
        }
        private async PromiseTask PreloadNodeFromLoading(IAssetKey assetKey)
        {
            await _parentNode.PreLoadChild(_treeNodes[assetKey]);
        }

        private async PromiseTask PreloadNodeFromRemoving(IAssetKey assetKey)
        {
            _treeNodes[assetKey].CancelUnload();
            await _parentNode.PreLoadChild(_treeNodes[assetKey]);
        }
        private async PromiseTask PreloadNodeFromUnloading(IAssetKey assetKey)
        {
            _treeNodes[assetKey].CancelUnload();
            OnUnloadNode(assetKey);
            await _parentNode.PreLoadChild(_treeNodes[assetKey]);
        }

        private async PromiseTask AddNodeFromStart(IAssetKey assetKey)
        {
            AssertUtil.IsFalse(_treeNodes.ContainsKey(assetKey), FoundationExceptionType.duplicate_addNode);

            var newNode = CreateNode(assetKey);
            _treeNodes.Add(assetKey, newNode);
            DispatchAddBegin(assetKey);

            await AddChildNode(assetKey, false);
        }
        private async PromiseTask AddNodeFromLoaded(IAssetKey assetKey, bool overwrite)
        {
            DispatchAddBegin(assetKey);

            await AddChildNode(assetKey, overwrite);
        }

        private async PromiseTask AddChildNode(IAssetKey assetKey, bool overwrite)
        {
            _focusNode = _treeNodes[assetKey];

            try
            {
                await _parentNode.AddChildAsync(_treeNodes[assetKey], overwrite);
            }
            catch 
            {
                OnFailLoadedNode(assetKey);
                throw;
            }

            OnAddLoadedNode(assetKey);
        }

        private void OnAddLoadedNode(IAssetKey assetKey)
        {
            OnAdded(assetKey);
            UnFreeze();
            DispatchAddEnd(assetKey);
        }

        private void OnFailLoadedNode(IAssetKey assetKey)
        {
            UnFreeze();
            DispatchAddEnd(assetKey);
            _treeNodes.Remove(assetKey);
        }

        private async PromiseTask AddNodeFromRemoving(IAssetKey assetKey)
        {
            _treeNodes[assetKey].CancelUnload();

            DispatchAddBegin(assetKey);
            await AddChildNode(assetKey, false);
        }

        private async PromiseTask AddNodeFromUnloading(IAssetKey assetKey)
        {
            var assetNode = _treeNodes[assetKey];
            assetNode.CancelUnload();

            // OnUnloadNode(assetKey);
            // await AddNodeFromStart(assetKey);

            //equivalent to
            await AddChildNode(assetKey, false);
        }

        protected virtual void DispatchSwitch(IAssetKey curNode, IAssetKey lastNode)
        {
        }
        protected virtual void DispatchAddBegin(IAssetKey targetNode)
        {
        }
        protected virtual void DispatchAddEnd(IAssetKey targetNode)
        {
        }
        protected virtual void DispatchRemoveBegin(IAssetKey targetNode)
        {
        }
        protected virtual void DispatchRemoveEnd(IAssetKey targetNode)
        {
        }


        #region Observer
        public void AddObserver<TEvent>(Package.EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.AddObserver(handler);
        }
        public void RemoveObserver<TEvent>(Package.EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.RemoveObserver(handler);
        }
        #endregion
    }
}
