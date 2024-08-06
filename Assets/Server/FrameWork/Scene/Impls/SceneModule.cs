using System.Collections.Generic;
using System.Threading.Tasks;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine.SceneManagement;
using Cr7Sund.Package.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneModule : LoadModule, ISceneModule
    {
        [Inject] private IGameNode _gameNode;

        public INode FocusScene => _focusNode;

        protected override INode _parentNode
        {
            get
            {
                return _gameNode;
            }
        }
        protected override long _loadTimeOutTime
        {
            get
            {
                return ServerBindDefine.SceneTimeOutTime;
            }
        }
        public int OperateNum
        {
            get
            {
                return _treeNodes.Count;
            }
        }

        public async PromiseTask PreLoadScene(IAssetKey key)
        {
            await CancelOtherScenes(key);
            await PreLoadNode(key);
        }

        public async PromiseTask AddScene(IAssetKey key)
        {
            await CancelOtherScenes(key);
            await AddNode(key);
            await CloseOtherScenes(key, LoadSceneMode.Single);
        }

        public async PromiseTask SwitchScene(IAssetKey key)
        {
            await AddScene(key);
            await CloseOtherScenes(key, LoadSceneMode.Additive);
        }

        public async PromiseTask UnloadScene(IAssetKey key)
        {
            await UnloadNode(key);
        }

        protected override async PromiseTask<INode> CreateNode(IAssetKey key)
        {
            var resultNode = await SceneCreator.Create((SceneKey)key);
            return resultNode as INode;
        }

        private async Task CancelOtherScenes(IAssetKey key)
        {
            var sceneKey = key as SceneKey;
            var loadingList = _poolBinder.AutoCreate<List<INode>>();
            var preloadedList = _poolBinder.AutoCreate<List<INode>>();
            foreach (var item in _treeNodes)
            {
                if (sceneKey == item.Key) continue;
                // if (sceneKey.LoadSceneMode != LoadSceneMode.Single) continue;

                if (item.Value.LoadState == LoadState.Loading)
                {
                    loadingList.Add(item.Value);
                }
                if (item.Value.NodeState == NodeState.Preloaded)
                {
                    preloadedList.Add(item.Value);
                }
            }

            for (int i = 0; i < loadingList.Count; i++)
            {
                Console.Warn("unload loading scene : {SceneNode} ", loadingList[i]);
                loadingList[i].CancelLoad();
            }

            for (int i = 0; i < preloadedList.Count; i++)
            {
                Console.Warn("unload preloaded but disable scene : {SceneNode} ", preloadedList[i]);
                await UnloadNode(preloadedList[i].Key);
            }

            _poolBinder.Return<List<INode>, INode>(preloadedList);
            _poolBinder.Return<List<INode>, INode>(loadingList);
        }

        private async Task CloseOtherScenes(IAssetKey key, LoadSceneMode closeSceneMode)
        {
            var tmpList = _poolBinder.AutoCreate<List<INode>>();
            foreach (var keyValuePair in _treeNodes)
            {
                var assetNode = keyValuePair.Value;
                var sceneKey = keyValuePair.Key as SceneKey;

                if (assetNode.Key == key) continue;
                if (sceneKey.LoadSceneMode != closeSceneMode) continue;

                tmpList.Add(assetNode);
            }

            for (int i = 0; i < tmpList.Count; i++)
            {
                var assetNode = tmpList[i];
                await UnloadNode(assetNode.Key);
                DispatchSwitch(key, assetNode.Key);
            }

            _poolBinder.Return<List<INode>, INode>(tmpList);
        }
        #region  Event
        protected override void DispatchSwitch(IAssetKey curScene, IAssetKey lastScene)
        {
            var e = new SwitchSceneEvent
            {
                LastScene = lastScene,
                CurScene = curScene
            };
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetScene, string guid)
        {
            var e = new AddSceneBeginEvent();
            e.TargetScene = targetScene;
            e.guid = guid;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetScene)
        {
            var e = new AddSceneEndEvent();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddFail(IAssetKey targetScene, string guid, bool isUnload = true)
        {
            var e = new AddSceneFailEvent();
            e.TargetScene = targetScene;
            e.IsUnload = isUnload;
            e.guid = guid;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetScene)
        {
            var e = new RemoveSceneBeginEvent();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetScene, bool isUnload)
        {
            var e = new RemoveSceneEndEvent();
            e.TargetScene = targetScene;
            e.IsUnload = isUnload;
            _eventBus.Dispatch(e);
        }

        #endregion

    }
}
