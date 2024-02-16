using System.Collections.Generic;
using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Performance;
using Cr7Sund.Server.Api;
using Cr7Sund.Touch.Api;
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


        internal bool IsInTransition
        {
            get;
            private set;
        }

        protected abstract INode _parentNode { get; }


        internal LoadModule()
        {
            _treeNodes = new Dictionary<IAssetKey, INode>();
        }


        public IPromise<INode> AddNode(IAssetKey key)
        {
            Freeze();
            if (!_parentNode.IsStarted)
            {
                UnFreeze();
                return Promise<INode>.Rejected(new MyException(
                    $"NodeModule.AddNode: Do not allow AddNode while ParentNode.Started is false! NodeName: {key}"));
            }
            // allowed load a bundle of node which mean the parent node maybe not enabled

            if (_treeNodes.TryGetValue(key, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Removing)
                {
                    UnFreeze();
                    return Promise<INode>.Rejected(new MyException(
                        $"NodeModule.AddNode: the asset is removing. NodeName: {key}"));
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    UnFreeze();
                    return Promise<INode>.Rejected(new MyException(
                        $"NodeModule.AddNode: the asset is unloading. NodeName: {key}"));
                }
                if (assetNode.NodeState == NodeState.Ready)
                {
                    Log.Warn($"NodeModule.AddNode: the asset is already on the nodeTree. NodeName: {key}");
                    UnFreeze();
                    return _treeNodes[key].LoadStatus;
                }
            }

            var preloadPromise = AddNodeFromPreload(key);
            if (preloadPromise != null)
                return preloadPromise;

            var loadingPromise = AddNodeFromPreLoading(key);
            if (loadingPromise != null)
                return loadingPromise;

            var addingPromise = AddNodeFromAdding(key);
            if (addingPromise != null)
                return addingPromise;

            var disActivePromise = AddNodeFromDisable(key);
            if (disActivePromise != null)
                return disActivePromise;

            return AddNodeFromStart(key); //Or Restart
        }
        public IPromise<INode> RemoveNode(IAssetKey key)
        {
            return UnloadNodeInternal(key, false);
        }

        internal IPromise<INode> PreLoadNode(IAssetKey key)
        {
            if (!_parentNode.IsStarted)
            {
                return Promise<INode>.Rejected(new MyException($"NodeModule.PreLoadNode: Do not allow PreLoadNode while parent node's Started is false! NodeName: {key}"));
            }

            if (_treeNodes.TryGetValue(key, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    return Promise<INode>.Rejected(new MyException
                        ($"NodeModule.PreLoadNode: Do not allow PreLoadNode while scene is unloading from node tree! NodeName: {key}"));
                }
                if (assetNode.NodeState == NodeState.Removed)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset has been removed.(which mean it has already load) NodeName: {key} ");
                    return _treeNodes[key].RemoveStatus;
                }
                if (assetNode.NodeState == NodeState.Removing)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset is removing.(which mean it has already load) NodeName: {key} ");
                    return _treeNodes[key].RemoveStatus;
                }
                if (assetNode.IsLoading())
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset is loading! NodeName: {key} ");
                    return assetNode.LoadStatus;
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset is adding! NodeName: {key} ");
                    return assetNode.AddStatus;
                }
                if (assetNode.NodeState == NodeState.Preloaded)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset is preloaded! NodeName: {key} ");
                    return assetNode.LoadStatus;
                }
                if (assetNode.NodeState == NodeState.Ready)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the asset is already on the nodeTree. NodeName: {key} ");
                    return assetNode.AddStatus;
                }
            }

            var newNode = CreateNode(key);
            newNode.StartLoad();
            _treeNodes.Add(key, newNode);
            return newNode.PreLoad(newNode).Then(OnPreloadNewNode);
        }
        internal IPromise<INode> UnloadNode(IAssetKey key)
        {
            return UnloadNodeInternal(key, true);
        }
        internal IPromise<INode> UnloadNodeInternal(IAssetKey key, bool unload)
        {
            Freeze();
            if (!_parentNode.IsStarted)
            {
                UnFreeze();
                return Promise<INode>.Rejected(new MyException(
                    $"NodeModule.RemoveNode: Do not allow unload while parentNode.Started is false! NodeName: {key}"));
            }
            if (_treeNodes.TryGetValue(key, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Unloaded)
                {
                    Log.Warn($"try to remove an unloaded node: {assetNode.Key}");
                    return assetNode.UnloadStatus;
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    Log.Warn($"try to remove an unloading node: {assetNode.Key}");
                    return assetNode.UnloadStatus;
                }
                if (assetNode.IsLoading())
                {
                    DispatchRemoveBegin(assetNode.Key);
                    return assetNode.CancelLoad()
                        .Then(OnUnloadNode);
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    Log.Warn($"NodeModule.RemoveNode: the asset is adding , but is not loading! NodeName: {key} ");
                    assetNode.AddStatus.Cancel();
                }

                if (unload == false)
                {
                    return RemoveNodeFromNodeTree(key);
                }
                else
                {
                    return UnloadNodeFromNodeTree(key);
                }
            }

            UnFreeze();
            return Promise<INode>.Rejected(new MyException(
                $"NodeModule.RemoveNode: Unhandled occasion...RemoveNode Fail.. NodeName: {key}"));
        }
        internal IPromise<INode> SwitchNode(IAssetKey key)
        {
            return AddNode(key).Then(OnSwitchNode);
        }
        
        
        protected virtual void Freeze()
        {
            IsInTransition = true;
            _fingerGesture.Freeze();
        }
        protected virtual void UnFreeze()
        {
            IsInTransition = false;
            _fingerGesture.UnFreeze();
        }
        protected abstract INode CreateNode(IAssetKey key);
        protected virtual IPromise<INode> OnAdded(INode node)
        {
            return Promise<INode>.Resolved(node);
        }

        private IPromise<INode> OnSwitchNode(INode curNode)
        {
            var promise = Promise<INode>.Resolved(curNode);

            if (_treeNodes.Count <= 1) return promise;

            _focusNode = curNode;
            
            foreach (var keyValuePair in _treeNodes)
            {
                var assetNode = keyValuePair.Value;

                if (assetNode == curNode) continue;
                if (!assetNode.IsActive) continue;

                promise = promise.Then(_ =>
                {
                    return RemoveNode(assetNode.Key).Then(prevNode =>
                    {
                        DispatchSwitch(curNode.Key, prevNode.Key);
                        return prevNode;
                    });
                });
            }

            return promise.Then(node =>
            {
                MemoryMonitor.CleanMemory();
                return node;
            });
        }
        private IPromise<INode> RemoveNodeFromNodeTree(IAssetKey key)
        {
            var removeNode = _treeNodes[key];

            if (removeNode.NodeState == NodeState.Removed)
            {
                UnFreeze();
                return removeNode.RemoveStatus;
            }
            if (removeNode.NodeState == NodeState.Removing)
            {
                return removeNode.RemoveStatus;
            }
            if (removeNode.NodeState == NodeState.Preloaded)
            {
                DispatchRemoveBegin(removeNode.Key);

                return removeNode.UnloadAsync(removeNode)
                    .Then(OnRemoveNode);
            }
            
            removeNode.StartUnload(false);
            return _parentNode.RemoveChildAsync(removeNode)
                .Then(OnRemoveNode);
        }
        private IPromise<INode> UnloadNodeFromNodeTree(IAssetKey key)
        {
            var unloadNode = _treeNodes[key];

            DispatchRemoveBegin(unloadNode.Key);
            unloadNode.StartUnload(true);

            return _parentNode.UnloadChildAsync(unloadNode)
                .Then(OnUnloadNode);
        }
        private INode OnRemoveNode(INode removeNode)
        {
            UnFreeze();
            removeNode.EndUnLoad(true);
            if (_focusNode != null && _focusNode.Key == removeNode.Key)
                _focusNode = null;

            DispatchRemoveEnd(removeNode.Key);
            return removeNode;
        }
        private INode OnUnloadNode(INode unloadNode)
        {
            var unloadKey = unloadNode.Key;

            UnFreeze();
            unloadNode.EndUnLoad(false);
            if (_focusNode != null && _focusNode.Key == unloadKey)
                _focusNode = null;

            _treeNodes.Remove(unloadKey);
            DispatchRemoveEnd(unloadKey);

            return unloadNode;
        }
        private INode OnPreloadNewNode(INode node)
        {
            node.EndPreload();
            return node;
        }
        private IPromise<INode> AddNodeFromStart(IAssetKey key)
        {
            var newNode = CreateNode(key);
            DispatchAddBegin(newNode.Key);

            newNode.StartLoad();
            _treeNodes.Add(key, newNode);

            return newNode
                .PreLoad(newNode)
                .Then(OnAddNewLoadedNode); //PLAN:  potential callback hell, replace with async
        }
        private IPromise<INode> AddNodeFromPreLoading(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var assetNode)
                || assetNode.NodeState != NodeState.Preloading)
            {
                return null;
            }

            DispatchAddBegin(assetNode.Key);
            assetNode.SetAdding();

            return _parentNode.AddChildAsync(assetNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromPreload(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var assetNode)
                || assetNode.NodeState != NodeState.Preloaded)
            {
                return null;
            }

            DispatchAddBegin(assetNode.Key);
            assetNode.SetAdding();

            return _parentNode.AddChildAsync(assetNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromDisable(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var assetNode)
                || assetNode.NodeState != NodeState.Removed)
            {
                return null;
            }

            DispatchAddBegin(assetNode.Key);
            assetNode.SetAdding();

            return _parentNode.AddChildAsync(assetNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromAdding(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var assetNode)
                || assetNode.NodeState != NodeState.Adding)
            {
                return null;
            }

            return assetNode.AddStatus;
        }
        private IPromise<INode> OnAddNewLoadedNode(INode node)
        {
            node.SetAdding();
            return _parentNode.AddChildAsync(node)
                .Then(OnAddLoadedNode);
        }
        private IPromise<INode> OnAddLoadedNode(INode node)
        {
            node.SetReady();
            UnFreeze();
            DispatchAddEnd(node.Key);
            _focusNode = node;

            return OnAdded(node);
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
        public void AddObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.AddObserver(handler);
        }
        public void RemoveObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.RemoveObserver(handler);
        }
        #endregion
    }
}
