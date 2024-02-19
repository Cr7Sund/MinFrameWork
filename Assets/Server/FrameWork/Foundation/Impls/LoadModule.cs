using System.Collections.Generic;
using Cr7Sund.PackageTest.EventBus.Api;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
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


        public IPromise<INode> AddNode(IAssetKey key, bool overwrite = false)
        {
            Freeze();
            if (!_parentNode.IsStarted)
            {
                UnFreeze();
                return Promise<INode>.Rejected(new MyException(
                    $"NodeModule.AddNode: Do not allow AddNode while ParentNode.Started is false! NodeName: {key}"));
            }
            // if (!_parentNode.IsActive)
            // allowed load a bundle of node which mean the parent node maybe not enabled

            // rejected if we are -ing
            // in case of unhandled case or duplicate operation
            if (_treeNodes.TryGetValue(key, out var assetNode))
            {
                if (assetNode.NodeState == NodeState.Removing)
                {
                    return AddNodeFromRemoving(assetNode, overwrite);
                }
                if (assetNode.NodeState == NodeState.Unloading)
                {
                    return AddNodeFromUnloading(assetNode, overwrite);
                }
                if (assetNode.NodeState == NodeState.Ready)
                {
                    Log.Warn($"NodeModule.AddNode: the asset is already on the nodeTree. NodeName: {key}");
                    UnFreeze();
                    return assetNode.LoadStatus;
                }
                if (assetNode.IsLoading())
                {
                    if (overwrite)
                    {
                        assetNode.LoadStatus.Cancel();
                    }
                    return assetNode.LoadStatus;
                }
                if (assetNode.NodeState == NodeState.Adding)
                {
                    if (overwrite)
                    {
                        assetNode.AddStatus.Cancel();
                    }
                    return assetNode.AddStatus;
                }
                if (assetNode.NodeState == NodeState.Preloaded)
                {
                    return AddNodeFromLoaded(assetNode);
                }
                if (assetNode.NodeState == NodeState.Removed)
                {
                    return AddNodeFromLoaded(assetNode);
                }
            }

            return AddNodeFromStart(key); //Or Restart
        }
        public IPromise<INode> RemoveNode(IAssetKey key)
        {
            return UnloadNodeInternal(key, false);
        }
        internal IPromise<INode> PreLoadNode(IAssetKey key)
        {
            AssertUtil.IsFalse(_treeNodes.ContainsKey(key), FoundationExceptionType.duplicate_preloadNode);

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
            _treeNodes.Add(key, newNode);
            return newNode.PreLoadAsync(newNode);
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

            return _parentNode.RemoveChildAsync(removeNode)
                .Then(OnRemoveNode);
        }
        private IPromise<INode> UnloadNodeFromNodeTree(IAssetKey key)
        {
            var unloadNode = _treeNodes[key];

            DispatchRemoveBegin(unloadNode.Key);

            return _parentNode.UnloadChildAsync(unloadNode)
                .Then(OnUnloadNode);
        }
        private INode OnRemoveNode(INode removeNode)
        {
            UnFreeze();
            if (_focusNode != null && _focusNode.Key == removeNode.Key)
                _focusNode = null;

            DispatchRemoveEnd(removeNode.Key);
            return removeNode;
        }
        private INode OnUnloadNode(INode unloadNode)
        {
            var unloadKey = unloadNode.Key;

            UnFreeze();
            if (_focusNode != null && _focusNode.Key == unloadKey)
                _focusNode = null;

            _treeNodes.Remove(unloadKey);
            DispatchRemoveEnd(unloadKey);

            return unloadNode;
        }

        private IPromise<INode> AddNodeFromStart(IAssetKey key)
        {
            AssertUtil.IsFalse(_treeNodes.ContainsKey(key), FoundationExceptionType.duplicate_addNode);

            var newNode = CreateNode(key);
            DispatchAddBegin(newNode.Key);

            _treeNodes.Add(key, newNode);

            return newNode
                .PreLoadAsync(newNode)
                .Then(OnAddNewLoadedNode); //PLAN:  potential callback hell, replace with async
        }
        private IPromise<INode> AddNodeFromLoaded(INode assetNode)
        {
            DispatchAddBegin(assetNode.Key);

            return _parentNode.AddChildAsync(assetNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromRemoving(INode assetNode, bool overwrite)
        {
            if (!overwrite)
            {
                UnFreeze();
                return Promise<INode>.Rejected(new MyException(
                    $"NodeModule.AddNode: the asset is removing. NodeName: {assetNode.Key}"));
            }
            else
            {
                assetNode.RemoveStatus.Cancel();

                DispatchAddBegin(assetNode.Key);

                return _parentNode.AddChildAsync(assetNode).Then(OnAddLoadedNode);
            }
        }
        private IPromise<INode> AddNodeFromUnloading(INode assetNode, bool overwrite)
        {
            if (!overwrite)
            {
                UnFreeze();
                return Promise<INode>.Rejected(new MyException(
                    $"NodeModule.AddNode: the asset is unloading. NodeName: {assetNode.Key}"));
            }
            else
            {
                assetNode.UnloadStatus.Cancel();
                return assetNode.UnloadStatus
                       .Then(OnUnloadNode)
                       .Then((node) => AddNodeFromStart(node.Key));
            }
        }
        private IPromise<INode> OnAddNewLoadedNode(INode node)
        {
            return _parentNode.AddChildAsync(node)
                .Then(OnAddLoadedNode);
        }
        private IPromise<INode> OnAddLoadedNode(INode node)
        {
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
