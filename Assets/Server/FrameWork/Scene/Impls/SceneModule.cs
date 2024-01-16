using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.EventBus;
using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Performance;
using Cr7Sund.Server.Apis;
using Cr7Sund.Touch.Api;
using NUnit.Framework.Constraints;

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
        private Dictionary<SceneKey, SceneNode> _treeScenes;
        private Dictionary<SceneKey, SceneNode> _preloadScenes;
        private Dictionary<SceneKey, SceneNode> _loadingScenes;
        private Dictionary<SceneKey, SceneNode> _addingScenes;
        private Dictionary<SceneKey, SceneNode> _removingScenes;
        private Dictionary<SceneKey, SceneNode> _unloadingScenes;


        public SceneNode FocusScene
        {
            get;
            private set;
        }


        public SceneModule()
        {
            _treeScenes = new Dictionary<SceneKey, SceneNode>();
            _preloadScenes = new Dictionary<SceneKey, SceneNode>();
            _loadingScenes = new Dictionary<SceneKey, SceneNode>();
            _addingScenes = new Dictionary<SceneKey, SceneNode>();
            _removingScenes = new Dictionary<SceneKey, SceneNode>();
            _unloadingScenes = new Dictionary<SceneKey, SceneNode>();
        }



        public IPromise<INode> PreLoadScene(SceneKey key)
        {
            if (!_gameNode.IsStarted)
            {
                return Promise<INode>.Rejected(new MyException($"SceneModule.PreLoadScene: Do not allow PreLoadScene while App.Started is false! SceneName: {key}"));
            }

            if (_addingScenes.ContainsKey(key))
            {
                return Promise<INode>.Rejected(new MyException($"SceneModule.PreLoadScene: Do not allow PreLoadScene while scene is removing from node tree! SceneName: {key}"));
            }
            if (_unloadingScenes.ContainsKey(key))
            {
                return Promise<INode>.Rejected(new MyException($"SceneModule.PreLoadScene: Do not allow PreLoadScene while scene is unloading from node tree! SceneName: {key}"));
            }
            if (_loadingScenes.ContainsKey(key))
            {
                Log.Warn($"SceneModule.PreLoadScene: the scene is loading! SceneName: {key} ");
                return _loadingScenes[key].LoadStatus;
            }
            if (_treeScenes.ContainsKey(key))
            {
                Log.Warn($"SceneModule.PreLoadScene: the scene is on the nodeTree. SceneName: {key} ");
                return _treeScenes[key].LoadStatus;
            }

            if (_preloadScenes.ContainsKey(key))
            {
                Log.Warn($"SceneModule.PreLoadScene: the scene is Loaded! SceneName: {key} ");
                return _preloadScenes[key].LoadStatus;
            }

            var newScene = SceneCreator.Create(key);
            _loadingScenes[key] = newScene;
            return newScene.PreLoadAsync(newScene).Then(OnPreloadNewScene );
        }

        public IPromise<INode> AddScene(SceneKey key)
        {
            _fingerGesture.Freeze();
            if (!_gameNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: Do not allow AddScene while App.Started is false! SceneName: {key}"));
            }
            if (_removingScenes.ContainsKey(key))
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: the scene is removing. SceneName: {key}"));
            }
            if (_unloadingScenes.ContainsKey(key))
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.AddScene: the scene is unloading. SceneName: {key}"));
            }
            if (_treeScenes.ContainsKey(key))
            {
                Log.Warn($"SceneModule.AddScene: the scene is already on the nodeTree. SceneName: {key}");
                _fingerGesture.UnFreeze();
                return _treeScenes[key].LoadStatus;
            }
            
            var preloadPromise = AddSceneFromPreload(key);
            if (preloadPromise != null)
                return preloadPromise;

            var loadingPromise = AddSceneFromLoading(key);
            if (loadingPromise != null)
                return loadingPromise;

            var addingPromise = AddSceneFromAdding(key);
            if (addingPromise != null)
                return addingPromise;

            return AddSceneFromStart(key);
        }

        public IPromise<INode> RemoveScene(SceneKey key)
        {
            return UnloadSceneInternal(key, false);
        }
        public IPromise<INode> UnloadScene(SceneKey key)
        {
            return UnloadSceneInternal(key, true);
        }
        public IPromise<INode> UnloadSceneInternal(SceneKey key, bool unload)
        {
            _fingerGesture.Freeze();
            if (!_gameNode.IsStarted)
            {
                _fingerGesture.UnFreeze();
                return Promise<INode>.Rejected(new MyException($"SceneModule.RemoveScene: Do not allow PreLoadScene while App.Started is false! SceneName: {key}"));
            }
            if (_removingScenes.TryGetValue(key, out SceneNode removingScene))
            {
                return removingScene.UnloadStatus;
            }
            if (_unloadingScenes.TryGetValue(key, out SceneNode unloadingScene))
            {
                return unloadingScene.UnloadStatus;
            }
            if (_loadingScenes.TryGetValue(key, out SceneNode loadingScene))
            {
                DispatchRemoveBegin(loadingScene.Key);
                _loadingScenes.Remove(key);
                loadingScene.LoadStatus.Cancel();

                DispatchRemoveEnd(loadingScene.Key);
                _fingerGesture.UnFreeze();
                return loadingScene.LoadStatus;
            }

            if (_preloadScenes.TryGetValue(key, out SceneNode loadedScene))
            {
                DispatchRemoveBegin(loadedScene.Key);
                _preloadScenes.Remove(key);

                loadingScene.LoadStatus.Cancel();

                DispatchRemoveEnd(loadingScene.Key);
                _fingerGesture.UnFreeze();
                return loadingScene.LoadStatus;
            }

            if (_addingScenes.TryGetValue(key, out SceneNode addingScene))
            {
                DispatchRemoveBegin(loadedScene.Key);
                _addingScenes.Remove(key);
                addingScene.LoadStatus.Cancel();

                DispatchRemoveEnd(addingScene.Key);
                _fingerGesture.UnFreeze();
                return addingScene.LoadStatus;
            }

            if (_treeScenes.TryGetValue(key, out SceneNode removeScene))
            {
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
        public IPromise<INode> SwitchScene(SceneKey key)
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
                if(sceneNode == curNode) continue;
                
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

        private IPromise<INode> RemoveSceneFromNodeTree(SceneKey key)
        {
            var removeScene = _treeScenes[key];

            DispatchRemoveBegin(removeScene.Key);
            _treeScenes.Remove(key);
            _removingScenes.Add(key, removeScene);
            return _gameNode.RemoveChildAsync(removeScene).Then(OnRemoveScene);
        }
        private IPromise<INode> UnloadSceneFromNodeTree(SceneKey key)
        {
            var unloadScene = _treeScenes[key];

            DispatchRemoveBegin(unloadScene.Key);
            _treeScenes.Remove(key);
            _unloadingScenes.Add(key, unloadScene);
            return _gameNode.UnloadChildAsync(unloadScene).Then(OnUnloadScene);
        }

        private INode OnUnloadScene(INode content)
        {
            var removeNode = content as SceneNode;

            _unloadingScenes.Remove(removeNode.Key);
            DispatchRemoveEnd(removeNode.Key);
            _fingerGesture.UnFreeze();
            return content;
        }
        private INode OnRemoveScene(INode content)
        {
            var removeNode = content as SceneNode;
            var key = removeNode.Key;

            if (!_removingScenes.ContainsKey(key))
            {
                return removeNode;
            }

            _removingScenes.Remove(key);
            _fingerGesture.UnFreeze();
            if (_focusScene != null && _focusScene.Key == key)
                _focusScene = null;
            removeNode.Dispose();
            DispatchRemoveEnd(removeNode.Key);

            return content;
        }

        private INode OnPreloadNewScene(INode node)
        {
            SceneNode newScene = node as SceneNode;
            SceneKey key = newScene.Key;

            if (_loadingScenes.ContainsKey(key))
            {
                _loadingScenes.Remove(key);
                _preloadScenes[key] = newScene;
            }

            return node;
        }
        private IPromise<INode> AddSceneFromStart(SceneKey key)
        {
            SceneNode newScene = SceneCreator.Create(key);
            DispatchAddBegin(newScene.Key);

            _loadingScenes[key] = newScene;

            return newScene
                          .PreLoadAsync(newScene)
                          .Then(OnAddNewLoadedScene);
        }
        private IPromise<INode> AddSceneFromLoading(SceneKey key)
        {
            if (!_loadingScenes.TryGetValue(key, out SceneNode loadingScene))
            {
                return null;
            }

            DispatchAddBegin(loadingScene.Key);

            _loadingScenes.Remove(key);
            _preloadScenes[key] = loadingScene;
            _addingScenes[key] = loadingScene;

            return _gameNode.AddChildAsync(loadingScene).Then(OnAddLoadedScene);
        }
        private IPromise<INode> AddSceneFromAdding(SceneKey key)
        {
            if (!_addingScenes.TryGetValue(key, out SceneNode addingScene))
            {
                return null;
            }

            return addingScene.LoadStatus;
        }
        private IPromise<INode> AddSceneFromPreload(SceneKey key)
        {
            if (!_preloadScenes.TryGetValue(key, out SceneNode loadedScene))
                return null;

            DispatchAddBegin(loadedScene.Key);
            _addingScenes[key] = loadedScene;

            return _gameNode.AddChildAsync(loadedScene).Then(OnAddPreloadScene);
        }
        private IPromise<INode> OnAddNewLoadedScene(INode node)
        {
            SceneNode newScene = node as SceneNode;
            SceneKey key = newScene.Key;
            if (!_loadingScenes.ContainsKey(key))
            {
                return node.LoadStatus;
            }

            _loadingScenes.Remove(key);
            _preloadScenes[key] = newScene;
            _addingScenes[key] = newScene;

            return _gameNode.AddChildAsync(newScene).Then(OnAddPreloadScene);
        }
        private INode OnAddLoadedScene(INode node)
        {
            SceneNode loadingScene = node as SceneNode;
            SceneKey key = loadingScene.Key;

            if (!_addingScenes.ContainsKey(key))
            {
                return node;
            }

            _fingerGesture.UnFreeze();
            DispatchAddEnd(loadingScene.Key);
            _focusScene = loadingScene;
            _addingScenes.Remove(key);

            _treeScenes[key] = loadingScene;

            return node;
        }
        private INode OnAddPreloadScene(INode node)
        {
            var loadedScene = node as SceneNode;
            SceneKey key = loadedScene.Key;

            if (!_addingScenes.ContainsKey(key))
            {
                return node;
            }

            _fingerGesture.UnFreeze();
            DispatchAddEnd(loadedScene.Key);
            _focusScene = loadedScene;
            _addingScenes.Remove(key);
            _preloadScenes.Remove(key);

            _treeScenes[key] = loadedScene;

            return node;
        }
        private void DispatchSwitch(SceneKey curScene, SceneKey lastScene)
        {
            var e = _poolBinder.AutoCreate<SwitchSceneEvent>();
            e.LastScene = lastScene;
            e.CurScene = curScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchAddBegin(SceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchAddEnd(SceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchRemoveBegin(SceneKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        private void DispatchRemoveEnd(SceneKey targetScene)
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
