using System.Collections.Generic;
using System.Linq;
using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Performance;
using Cr7Sund.Server.Apis;
using Cr7Sund.Touch.Api;

namespace Cr7Sund.Server.Impl
{
    public class SceneModule : ISceneModule
    {
        [Inject]
        private IFingerGesture _fingerGesture;
        [Inject]
        private GameNode _gameNode;
        [Inject]
        private IEventBus _eventBus;
        [Inject]
        private IPoolBinder _poolBinder;
        private SceneNode _focusScene;
        private Dictionary<ISceneKey, SceneNode> _treeScenes;


        public INode FocusScene
        {
            get;
            private set;
        }


        public SceneModule()
        {
            _treeScenes = new Dictionary<ISceneKey, SceneNode>();
        }


        public IPromise<INode> PreLoadScene(ISceneKey key)
        {
            if (!_gameNode.IsStarted)
            {
                return Promise<INode>.Rejected(new MyException($"SceneModule.PreLoadScene: Do not allow PreLoadScene while App.Started is false! SceneName: {key}"));
            }

            if (_treeScenes.TryGetValue(key, out var sceneNode))
            {
                if (sceneNode.NodeState == NodeState.Removed)
                {
                    Log.Warn($"SceneModule.PreLoadScene: the scene has been removed. SceneName: {key} ");
                    return _treeScenes[key].LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Unloading)
                {
                    return Promise<INode>.Rejected(new MyException
                            ($"SceneModule.PreLoadScene: Do not allow PreLoadScene while scene is unloading from node tree! SceneName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Removing)
                {
                    return Promise<INode>.Rejected(new MyException
                            ($"SceneModule.PreLoadScene: Do not allow PreLoadScene while scene is removing from node tree! SceneName: {key}"));
                }
                if (sceneNode.IsLoading())
                {
                    Log.Warn($"SceneModule.PreLoadScene: the scene is loading! SceneName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Adding)
                {
                    Log.Warn($"SceneModule.PreLoadScene: the scene is adding! SceneName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Preloaded)
                {
                    Log.Warn($"SceneModule.PreLoadScene: the scene is preloaded! SceneName: {key} ");
                    return sceneNode.LoadStatus;
                }
                if (sceneNode.NodeState == NodeState.Ready)
                {
                    return Promise<INode>.Rejected(new MyException
                             ($"SceneModule.PreLoadScene: the scene is on the nodeTree. SceneName: {key} "));
                }
            }

            var newScene = SceneCreator.Create((SceneKey)key);
            newScene.StartLoad();
            _treeScenes[key] = newScene;
            return newScene.PreLoad(newScene).Then(OnPreloadNewScene);
        }
        public IPromise<INode> AddScene(ISceneKey key)
        {
            _fingerGesture.Freeze();
            if (!_gameNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: Do not allow AddScene while App.Started is false! SceneName: {key}"));
            }

            if (_treeScenes.TryGetValue(key, out var sceneNode))
            {
                if (sceneNode.NodeState == NodeState.Removing)
                {
                    _fingerGesture.UnFreeze();
                    return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: the scene is removing. SceneName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Unloading)
                {
                    _fingerGesture.UnFreeze();
                    return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: the scene is unloading. SceneName: {key}"));
                }
                if (sceneNode.NodeState == NodeState.Ready)
                {
                    Log.Warn($"SceneModule.AddScene: the scene is already on the nodeTree. SceneName: {key}");
                    _fingerGesture.UnFreeze();
                    return _treeScenes[key].LoadStatus;
                }
            }

            var preloadPromise = AddSceneFromPreload(key);
            if (preloadPromise != null)
                return preloadPromise;

            var loadingPromise = AddSceneFromPreLoading(key);
            if (loadingPromise != null)
                return loadingPromise;

            var addingPromise = AddSceneFromAdding(key);
            if (addingPromise != null)
                return addingPromise;

            var disActivePromise = AddSceneFromDisable(key);
            if (disActivePromise != null)
                return disActivePromise;

            return AddSceneFromStart(key);//Or Restart
        }
        public IPromise<INode> RemoveScene(ISceneKey key)
        {
            return UnloadSceneInternal(key, false);
        }
        public IPromise<INode> UnloadScene(ISceneKey key)
        {
            return UnloadSceneInternal(key, true);
        }
        public IPromise<INode> UnloadSceneInternal(ISceneKey key, bool unload)
        {
            _fingerGesture.Freeze();
            if (!_gameNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.RemoveScene: Do not allow PreLoadScene while App.Started is false! SceneName: {key}"));
            }
            if (_treeScenes.TryGetValue(key, out var sceneNode))
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
                                DispatchRemoveEnd(((SceneNode)node).Key);
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
                                DispatchRemoveEnd(((SceneNode)node).Key);
                                _fingerGesture.UnFreeze();
                                return node;
                            });
                }
                if (sceneNode.NodeState == NodeState.Preloaded)
                {
                    DispatchRemoveBegin(sceneNode.Key);

                    return sceneNode.UnloadAsync(sceneNode).Then(node =>
                    {
                        var sNode = (SceneNode)node;
                        _treeScenes.Remove(sNode.Key);
                        DispatchRemoveEnd(sNode.Key);
                        _fingerGesture.UnFreeze();
                        return node;
                    });
                }

                if (unload == false)
                {
                    return RemoveSceneFromNodeTree(key);
                }
                else
                {
                    return UnloadSceneFromNodeTree(key);
                }
            }
            else
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.RemoveScene: Can't find scene...RemoveScene Fail.. SceneName: {key}"));
            }
        }
        public IPromise<INode> SwitchScene(ISceneKey key)
        {
            return AddScene(key).Then(OnSwitchScene);
        }

        private IPromise<INode> OnSwitchScene(INode curNode)
        {
            var curScene = curNode as SceneNode;
            var promise = Promise<INode>.Resolved(curNode);

            if (_treeScenes.Count <= 1) return promise;

            FocusScene = curScene;

            var tmpList = _treeScenes.Values.ToArray();
            for (int i = tmpList.Length - 1; i >= 0; i--)
            {
                var sceneNode = tmpList[i];
                if (sceneNode == curNode) continue;

                promise = promise.Then(_ =>
                   {
                       return RemoveScene(sceneNode.Key).Then((prevNode) =>
                          {
                              var prevScene = prevNode as SceneNode;
                              DispatchSwitch(curScene.Key, prevScene.Key);
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
        private IPromise<INode> RemoveSceneFromNodeTree(ISceneKey key)
        {
            var removeScene = _treeScenes[key];

            DispatchRemoveBegin(removeScene.Key);
            removeScene.StartUnload(false);
            return _gameNode.RemoveChildAsync(removeScene).Then(OnRemoveScene);
        }
        private IPromise<INode> UnloadSceneFromNodeTree(ISceneKey key)
        {
            var unloadScene = _treeScenes[key];

            DispatchRemoveBegin(unloadScene.Key);
            unloadScene.StartUnload(true);

            return _gameNode.UnloadChildAsync(unloadScene).Then(OnUnloadScene);
        }
        private INode OnRemoveScene(INode content)
        {
            var removeNode = content as SceneNode;

            _fingerGesture.UnFreeze();
            removeNode.EndLoad(true);
            if (_focusScene != null && _focusScene.Key == removeNode.Key)
                _focusScene = null;

            DispatchRemoveEnd(removeNode.Key);
            return content;
        }
        private INode OnUnloadScene (INode content)
        {
            var removeNode = content as SceneNode;
            var key = removeNode.Key;

            _fingerGesture.UnFreeze();
            removeNode.EndLoad(false);
            if (_focusScene != null && _focusScene.Key == key)
                _focusScene = null;

            _treeScenes.Remove(removeNode.Key);
            DispatchRemoveEnd(removeNode.Key);

            return content;
        }
        private INode OnPreloadNewScene(INode node)
        {
            SceneNode newScene = node as SceneNode;
            ISceneKey key = newScene.Key;

            newScene.EndPreload();

            return node;
        }
        private IPromise<INode> AddSceneFromStart(ISceneKey key)
        {
            SceneNode newScene = SceneCreator.Create((SceneKey)key);
            DispatchAddBegin(newScene.Key);

            newScene.StartLoad();
            _treeScenes[key] = newScene;

            return newScene
                          .PreLoad(newScene)
                          .Then(OnAddNewLoadedScene); //PLAN:  potential callback hell, replace with async
        }
        private IPromise<INode> AddSceneFromPreLoading(ISceneKey key)
        {
            if (!_treeScenes.TryGetValue(key, out var sceneNode)
                 || (sceneNode.NodeState != NodeState.Preloading))
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _gameNode.AddChildAsync(sceneNode).Then(OnAddLoadedScene);
        }
        private IPromise<INode> AddSceneFromPreload(ISceneKey key)
        {
            if (!_treeScenes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Preloaded)
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _gameNode.AddChildAsync(sceneNode).Then(OnAddLoadedScene);
        }
        private IPromise<INode> AddSceneFromDisable(ISceneKey key)
        {
            if (!_treeScenes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Removed)
            {
                return null;
            }

            DispatchAddBegin(sceneNode.Key);
            sceneNode.SetAdding();

            return _gameNode.AddChildAsync(sceneNode).Then(OnAddLoadedScene);
        }
        private IPromise<INode> AddSceneFromAdding(ISceneKey key)
        {
            if (!_treeScenes.TryGetValue(key, out var sceneNode)
                || sceneNode.NodeState != NodeState.Adding)
            {
                return null;
            }

            return sceneNode.LoadStatus;
        }

        private IPromise<INode> OnAddNewLoadedScene(INode node)
        {
            SceneNode newScene = node as SceneNode;
            ISceneKey key = newScene.Key;

            newScene.SetAdding();

            return _gameNode.AddChildAsync(newScene).Then(OnAddLoadedScene);
        }
        private INode OnAddLoadedScene(INode node)
        {
            SceneNode loadingScene = node as SceneNode;
            ISceneKey key = loadingScene.Key;

            loadingScene.SetReady();
            _fingerGesture.UnFreeze();
            DispatchAddEnd(loadingScene.Key);
            _focusScene = loadingScene;

            return node;
        }
        private void DispatchSwitch(ISceneKey curScene, ISceneKey lastScene)
        {
            var e = _poolBinder.AutoCreate<SwitchSceneEvent>();
            e.LastScene = lastScene;
            e.CurScene = curScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchAddBegin(ISceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchAddEnd(ISceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchRemoveBegin(ISceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchRemoveEnd(ISceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }

        public void AddObserver<TEvent>(EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.AddObserver<TEvent>(handler);
        }

        public void RemoveObserver<TEvent>(EventBus.Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            _eventBus.RemoveObserver<TEvent>(handler);
        }

    }
}
