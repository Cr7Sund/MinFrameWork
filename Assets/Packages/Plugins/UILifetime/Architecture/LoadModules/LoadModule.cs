using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.LifeTime;
using Object = UnityEngine.Object;

namespace Cr7Sund.LifeTime
{
    public  class LoadModule : ILoadModule
    {
        [Inject] IUniqueInstanceContainer _uiUniqueContainer;
        [Inject] IInstancesContainer _uiContainer;
        [Inject] private ISceneLoader _sceneLoader;
         
        protected Dictionary<IRouteKey, INode> _treeNodes;

        public LoadModule()
        {
            _treeNodes = new Dictionary<IRouteKey, INode>();
        }


        #region Lifecycle Methods
        public async PromiseTask StartAdd(IRouteKey parentKey, IRouteKey assetKey, IRouteArgs fragmentContext)
        {
            try
            {
                var isStarted = await TryAddNodeFromStarted(assetKey);

                if (!isStarted
                    && !_treeNodes.ContainsKey(assetKey))
                {
                    var rootNode = _treeNodes[parentKey]; // the first level node is GameNode
                    DispatchAddBegin(assetKey, GetNodeGuid(assetKey));

                    var assetNode = assetKey.CreateNode();
                    await assetNode.StartCreate(fragmentContext, rootNode);
                    AddTreeNode(assetKey, assetNode);
                }

            }
            catch (Exception ex)
            {
                OnFailLoadedNode(assetKey, ex);
                throw;
            }
        }

        public async PromiseTask Load(IRouteKey assetKey)
        {
            try
            {
                var treeNode = _treeNodes[assetKey];
                var loadAssetTask = LoadAssetAsync(assetKey, treeNode.AddCancellation.Token);
                await loadAssetTask;
              await  treeNode.EndLoad();
            }
            catch (Exception ex)
            {
                OnFailLoadedNode(assetKey, ex);
                throw;
            }
        }

        public async PromiseTask Preload(IRouteKey assetKey, IRouteArgs fragmentContext)
        {
            var treeNode = _treeNodes[assetKey];
            var preloadTask = treeNode.PreloadView(fragmentContext);
            await preloadTask;
        }

        public async PromiseTask Attach(IRouteKey assetKey, IRouteArgs fragmentContext)
        {
            var treeNode = _treeNodes[assetKey];
            await treeNode.Start(treeNode.AddCancellation.Token);
        }

        public async PromiseTask AppendTransition(IRouteKey assetKey)
        {
            var treeNode = _treeNodes[assetKey];
            await treeNode.AppendTransition();
        }

        public async PromiseTask Visible(IRouteKey assetKey)
        {
            var treeNode = _treeNodes[assetKey];
            await treeNode.Enable();
        }

        public void EndAdd(IRouteKey assetKey)
        {
            if (_treeNodes.TryGetValue(assetKey, out var treeNode))
            {
                AssertUtil.IsTrue(treeNode.IsStarted & treeNode.IsActive);
                _treeNodes[assetKey].EndCreate();
                DispatchAddEnd(assetKey);
            }
        }

        public async PromiseTask StartRemove(IRouteKey assetKey, bool isRemove)
        {
            AssertUtil.IsTrue(_treeNodes.ContainsKey(assetKey), $"try to remove a node that is not in tree {assetKey}");

            var isStarted = await TryRemoveNodeFromStarted(assetKey);

            if (!isStarted
                && _treeNodes.ContainsKey(assetKey))
            {
                var treeNode = _treeNodes[assetKey];
                DispatchRemoveBegin(assetKey);
                treeNode.StartDestroy(isRemove);
            }
        }

        public async PromiseTask Invisible(IRouteKey assetKey)
        {
            var treeNode = _treeNodes[assetKey];
            await treeNode.Disable();
        }

        public async PromiseTask DisAttach(IRouteKey assetKey, bool isRemove)
        {
            var treeNode = _treeNodes[assetKey];
            if (!isRemove)
            {
                await treeNode.Stop(treeNode.RemoveCancellation.Token);
                try
                {
                    UnloadAsset(assetKey, treeNode.RemoveCancellation.Token);
                }
                catch (Exception e)
                {
                    OnFailUnLoadedNode(assetKey, e);
                    throw;
                }
                treeNode.Destroy();
                RemoveTreeNode(assetKey);
            }
        }

        public void EndRemove(IRouteKey assetKey, bool isRemove)
        {
            if (_treeNodes.TryGetValue(assetKey, out var treeNode))
            {
                bool keepInStack = isRemove;
                AssertUtil.IsFalse(!keepInStack && treeNode.IsStarted);
                AssertUtil.IsFalse(treeNode.IsActive);

                treeNode.EndDestroy(isRemove);
                DispatchRemoveEnd(assetKey, !keepInStack);
            }
        }

        public virtual void Dispose()
        {
            if (MacroDefine.IsDebug)
            {
                foreach (var item in _treeNodes)
                {
                    Console.Info("LoadModule: Still left {Node}", item.Key);
                }
            }

            AssertUtil.LessOrEqual(_treeNodes.Count, 0);
            _treeNodes = null;
        }
#endregion

        #region Private Methods
        private async PromiseTask<bool> TryAddNodeFromStarted(IRouteKey assetKey)
        {
            if (!_treeNodes.TryGetValue(assetKey, out var assetNode))
            {
                return false;
            }

            switch (assetNode.NodeState)
            {
                case NodeState.Removing:
                case NodeState.Unloading:
                    Console.Warn("NodeModule.LoadNode: the asset is {NodeState}! NodeName: {assetKey} ", assetNode.NodeState, assetKey);
                    _treeNodes[assetKey].CancelUnLoadNode();
                    return false;
                case NodeState.Adding:
                    Console.Warn("NodeModule.LoadNode: the asset is {NodeState}! NodeName: {assetKey} ", assetNode.NodeState, assetKey);
                    if (assetKey.OverwriteTask)
                    {
                        assetNode.CancelAddNode();
                        return false;
                    }
                    else
                    {
                        await assetNode.AddStatus.Task;
                        return true;
                    }
                default:
                    return false;
                //NodeState.Preloaded 
                //NodeState.Removed
                // NodeState.Default
            }
        }

        private async PromiseTask<bool> TryRemoveNodeFromStarted(IRouteKey assetKey)
        {
            if (!_treeNodes.TryGetValue(assetKey, out var assetNode))
            {
                return false;
            }

            switch (assetNode.NodeState)
            {
                case NodeState.Removing:
                case NodeState.Unloading:
                    Console.Warn("NodeModule.LoadNode: the asset is {NodeState}! NodeName: {assetKey} ", assetNode.NodeState, assetKey);
                    if (assetKey.OverwriteTask)
                    {
                        assetNode.CancelUnLoadNode();
                        return false;
                    }
                    else
                    {
                        await assetNode.RemoveStatus.Task;
                        return true;
                    }
                case NodeState.Adding:
                    Console.Warn("NodeModule.LoadNode: the asset is {NodeState}! NodeName: {assetKey} ", assetNode.NodeState, assetKey);
                    _treeNodes[assetKey].CancelAddNode();
                    return false;
                default: return false;
                //NodeState.Preloaded 
                //NodeState.Removed
                // NodeState.Default
            }
        }

        private void OnFailLoadedNode(IRouteKey assetKey, Exception ex)
        {
            DispatchAddFail(assetKey, GetNodeGuid(assetKey), true);
            RemoveTreeNode(assetKey);
        }

        private void OnFailUnLoadedNode(IRouteKey assetKey, Exception ex)
        {
            DispatchRemoveFail(assetKey, GetNodeGuid(assetKey), true);
            RemoveTreeNode(assetKey);
        }

        private void AddTreeNode(IRouteKey assetKey, INode treeNode)
        {
            _treeNodes.Add(assetKey, treeNode);
        }

        private void RemoveTreeNode(IRouteKey assetKey)
        {
            if (_treeNodes.ContainsKey(assetKey))
                _treeNodes.Remove(assetKey);
        }

#endregion

        #region Load methods

        private async PromiseTask LoadAssetAsync(IRouteKey assetKey, UnsafeCancellationToken cancellation)
        {
            if (assetKey is SceneKey sceneKey)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad, cancellation);
                await _sceneLoader.ActiveSceneAsync(sceneKey);
            }
            else if (assetKey is UIKey uiKey)
            {
                if (uiKey.UniqueInstance)
                {
                    await _uiUniqueContainer.InstantiateAsync<Object>(uiKey, cancellation);
                }
                else
                {
                    await _uiContainer.InstantiateAsync<Object>(uiKey, uiKey.PanelID, cancellation);
                }
            }
        }

        private void UnloadAsset(IRouteKey assetKey, UnsafeCancellationToken cancellation)
        {
            if (assetKey is SceneKey sceneKey)
            {
                _sceneLoader.UnloadScene(sceneKey);
            }
            else if (assetKey is UIKey uiKey)
            {
                if (uiKey.UniqueInstance)
                {
                    _uiUniqueContainer.Unload(assetKey);
                }
                else
                {
                    _uiContainer.ReturnInstance(uiKey.PanelID, assetKey);
                }
            }

        }

        protected virtual string GetNodeGuid(IRouteKey assetKey)
        {
            if (assetKey is UIKey uiKey)
            {
                if (!uiKey.UniqueInstance)
                {
                    return uiKey.PanelID;
                }
            }
            return assetKey.Key;
        }

                #region Events
        protected virtual void DispatchAddBegin(IRouteKey targetNode, string guid)
        {
        }
        protected virtual void DispatchAddEnd(IRouteKey targetNode)
        {
        }

        protected virtual void DispatchAddFail(IRouteKey targetNode, string guid, bool isUnload = true)
        {
        }
        protected virtual void DispatchRemoveFail(IRouteKey targetNode, string guid, bool isUnload = true)
        {
        }
        protected virtual void DispatchRemoveBegin(IRouteKey targetNode)
        {
        }
        protected virtual void DispatchRemoveEnd(IRouteKey targetNode, bool isUnload)
        {
        }
        #endregion
          #endregion

    }
}
