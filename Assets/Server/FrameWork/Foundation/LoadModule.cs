using System.Collections.Generic;
using System.Linq;
using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Performance;
using Cr7Sund.Touch.Api;

namespace Cr7Sund.Server.Impl
{
    public abstract class LoadModule 
    {
        [Inject]
        protected INode _parentNode;
        [Inject]
        private IFingerGesture _fingerGesture;
        [Inject]
        protected IEventBus _eventBus;
        [Inject]
        protected IPoolBinder _poolBinder;
        protected Dictionary<IAssetKey, INode> _treeNodes;
        private INode _focusNode;


        public INode FocusNode
        {
            get;
            private set;
        }


        public LoadModule()
        {
            _treeNodes = new Dictionary<IAssetKey, INode>();
        }


        public IPromise<INode> PreLoadNode(IAssetKey key)
        {
            if (!_parentNode.IsStarted)
            {
                return Promise<INode>.Rejected(new MyException($"NodeModule.PreLoadNode: Do not allow PreLoadNode while App.Started is false! NodeName: {key}"));
            }

            if (_treeNodes.TryGetValue(key, out var sceneNode))
            {
                if (sceneNode.NodeState == NodeState.Removed)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the scene has been removed. NodeName: {key} ");
                    return _treeNodes[key].LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Unloading)
                {
                    return Promise<INode>.Rejected(new MyException
                            ($"NodeModule.PreLoadNode: Do not allow PreLoadNode while scene is unloading from node tree! NodeName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Removing)
                {
                    return Promise<INode>.Rejected(new MyException
                            ($"NodeModule.PreLoadNode: Do not allow PreLoadNode while scene is removing from node tree! NodeName: {key}"));
                }
                if (sceneNode.IsLoading())
                {
                    Log.Warn($"NodeModule.PreLoadNode: the scene is loading! NodeName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Adding)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the scene is adding! NodeName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Preloaded)
                {
                    Log.Warn($"NodeModule.PreLoadNode: the scene is preloaded! NodeName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Ready)
                {
                    return Promise<INode>.Rejected(new MyException
                             ($"NodeModule.PreLoadNode: the scene is on the nodeTree. NodeName: {key} "));
                }
            }

            var newNode = CreateNode(key);
            newNode.StartLoad();
            _treeNodes[key] = newNode;
            return newNode.PreLoad(newNode).Then(OnPreloadNewNode);
        }
        public IPromise<INode> AddNode(IAssetKey key)
        {
            _fingerGesture.Freeze();
            if (!_parentNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"NodeModule.AddNode: Do not allow AddNode while App.Started is false! NodeName: {key}"));
            }

            if (_treeNodes.TryGetValue(key, out var sceneNode))
            {
                if (sceneNode.NodeState == NodeState.Removing)
                {
                    _fingerGesture.UnFreeze();
                    return Promise<INode>.Rejected(new MyException($"NodeModule.AddNode: the scene is removing. NodeName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Unloading)
                {
                    _fingerGesture.UnFreeze();
                    return Promise<INode>.Rejected(new MyException($"NodeModule.AddNode: the scene is unloading. NodeName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Ready)
                {
                    Log.Warn($"NodeModule.AddNode: the scene is already on the nodeTree. NodeName: {key}");
                    _fingerGesture.UnFreeze();
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

            return AddNodeFromStart(key);//Or Restart
        }
        public IPromise<INode> RemoveNode(IAssetKey key)
        {
            return UnloadNodeInternal(key, false);
        }
        public IPromise<INode> UnloadNode(IAssetKey key)
        {
            return UnloadNodeInternal(key, true);
        }
        public IPromise<INode> UnloadNodeInternal(IAssetKey key, bool unload)
        {
            _fingerGesture.Freeze();
            if (!_parentNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"NodeModule.RemoveNode: Do not allow PreLoadNode while App.Started is false! NodeName: {key}"));
            }
            if (_treeNodes.TryGetValue(key, out var sceneNode))
            {
                if (sceneNode.NodeState == NodeState.Removing)
                {
                    return sceneNode.UnloadStatus;
                }
                if (sceneNode.NodeState == NodeState.Unloading)
                {
                    return sceneNode.UnloadStatus;
                }
                if (sceneNode.IsLoading())
                {
                    DispatchRemoveBegin(sceneNode.Key);
                    return sceneNode.CancelLoad()
                            .Then(node =>
                            {
                                DispatchRemoveEnd(((INode)node).Key);
                                _fingerGesture.UnFreeze();
                                return node;
                            });
                }
                if (sceneNode.NodeState == NodeState.Adding)
                {
                    DispatchRemoveBegin(sceneNode.Key);
                    return sceneNode.CancelLoad()
                            .Then(node =>
                            {
                                DispatchRemoveEnd(((INode)node).Key);
                                _fingerGesture.UnFreeze();
                                return node;
                            });
                }
                if (sceneNode.NodeState == NodeState.Preloaded)
                {
                    DispatchRemoveBegin(sceneNode.Key);

                    return sceneNode.UnloadAsync(sceneNode).Then(node =>
                    {
                        var sNode = (INode)node;
                        _treeNodes.Remove(sNode.Key);
                        DispatchRemoveEnd(sNode.Key);
                        _fingerGesture.UnFreeze();
                        return node;
                    });
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
            else
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"NodeModule.RemoveNode: Can't find scene...RemoveNode Fail.. NodeName: {key}"));
            }
        }
        public IPromise<INode> SwitchNode(IAssetKey key)
        {
            return AddNode(key).Then(OnSwitchNode);
        }

        protected IPromise<INode> OnSwitchNode(INode curNode)
        {
            var promise = Promise<INode>.Resolved(curNode);

            if (_treeNodes.Count <= 1) return promise;

            FocusNode = curNode;

            var tmpList = _treeNodes.Values.ToArray();
            for (int i = tmpList.Length - 1; i >= 0; i--)
            {
                var sceneNode = tmpList[i];
                if (sceneNode == curNode) continue;

                promise = promise.Then(_ =>
                   {
                       return RemoveNode(sceneNode.Key).Then((prevNode) =>
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

            DispatchRemoveBegin(removeNode.Key);
            removeNode.StartUnload(false);
            return _parentNode.RemoveChildAsync(removeNode).Then(OnRemoveNode);
        }
        private IPromise<INode> UnloadNodeFromNodeTree(IAssetKey key)
        {
            var unloadNode = _treeNodes[key];

            DispatchRemoveBegin(unloadNode.Key);
            unloadNode.StartUnload(true);

            return _parentNode.UnloadChildAsync(unloadNode).Then(OnUnloadNode);
        }
        private INode OnRemoveNode(INode content)
        {
            var removeNode = content as INode;

            _fingerGesture.UnFreeze();
            removeNode.EndLoad(true);
            if (_focusNode != null && _focusNode.Key == removeNode.Key)
                _focusNode = null;

            DispatchRemoveEnd(removeNode.Key);
            return content;
        }
        private INode OnUnloadNode(INode content)
        {
            var removeNode = content as INode;
            var key = removeNode.Key;

            _fingerGesture.UnFreeze();
            removeNode.EndLoad(false);
            if (_focusNode != null && _focusNode.Key == key)
                _focusNode = null;

            _treeNodes.Remove(removeNode.Key);
            DispatchRemoveEnd(removeNode.Key);

            return content;
        }
        private INode OnPreloadNewNode(INode node)
        {
            INode newNode = node as INode;
            IAssetKey key = newNode.Key;

            newNode.EndPreload();

            return node;
        }
        private IPromise<INode> AddNodeFromStart(IAssetKey key)
        {
            INode newNode = CreateNode(key);
            DispatchAddBegin(newNode.Key);

            newNode.StartLoad();
            _treeNodes[key] = newNode;

            return newNode
                          .PreLoad(newNode)
                          .Then(OnAddNewLoadedNode); //PLAN:  potential callback hell, replace with async
        }
        private IPromise<INode> AddNodeFromPreLoading(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var sceneNode)
                 || (sceneNode.NodeState != NodeState.Preloading))
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _parentNode.AddChildAsync(sceneNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromPreload(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Preloaded)
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _parentNode.AddChildAsync(sceneNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromDisable(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Removed)
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _parentNode.AddChildAsync(sceneNode).Then(OnAddLoadedNode);
        }
        private IPromise<INode> AddNodeFromAdding(IAssetKey key)
        {
            if (!_treeNodes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Adding)
            {
                return null;
            }

            return sceneNode.LoadStatus;
        }
        private IPromise<INode> OnAddNewLoadedNode(INode node)
        {
            INode newNode = node as INode;
            IAssetKey key = newNode.Key;

            newNode.SetAdding();

            return _parentNode.AddChildAsync(newNode).Then(OnAddLoadedNode);
        }
        private INode OnAddLoadedNode(INode node)
        {
            INode loadingNode = node as INode;
            IAssetKey key = loadingNode.Key;

            loadingNode.SetReady();
            _fingerGesture.UnFreeze();
            DispatchAddEnd(loadingNode.Key);
            _focusNode = loadingNode;

            return node;
        }

        protected virtual  void DispatchSwitch(IAssetKey curNode, IAssetKey lastNode)
        {
        }
        protected virtual void DispatchAddBegin(IAssetKey targetNode)
        {
        }
        protected virtual void DispatchAddEnd(IAssetKey targetNode)
        {
        }
        protected virtual  void DispatchRemoveBegin(IAssetKey targetNode)
        {
        }
        protected virtual void DispatchRemoveEnd(IAssetKey targetNode)
        {
        }

        public void AddObserver<TEvent>(EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.AddObserver<TEvent>(handler);
        }

        public void RemoveObserver<TEvent>(EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.RemoveObserver<TEvent>(handler);
        }


        protected abstract INode CreateNode(IAssetKey key);
    }
}
