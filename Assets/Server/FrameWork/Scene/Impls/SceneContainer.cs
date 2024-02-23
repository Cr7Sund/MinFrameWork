using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneContainer : ISceneContainer
    {
        [Inject] IAssetLoader _assetLoader;

        private Dictionary<string, GameObject> _sceneInstanceContainers = new();
        private Dictionary<string, IAssetPromise> _scenePromiseContainers = new();

        public string SceneName { get; set; }

        public GameObject CreateInstance(string name, params Type[] components)
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                var scene = SceneManager.GetSceneByName(SceneName);
                AssertUtil.NotNull(scene, SceneExceptionType.create_from_invalidScene);

                SceneManager.SetActiveScene(scene);
                instance = new GameObject(name, components);
                _sceneInstanceContainers.Add(name, instance);
            }

            return instance;
        }

        public T CreateInstanceWithComponent<T>(string name) where T : Object
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                var scene = SceneManager.GetSceneByName(SceneName);
                AssertUtil.NotNull(scene, SceneExceptionType.create_from_invalidScene);

                SceneManager.SetActiveScene(scene);
                instance = new GameObject(name, typeof(T));
                _sceneInstanceContainers.Add(name, instance);
            }

            return instance.GetComponent<T>();
        }

        public GameObject LoadInstance(IAssetKey assetKey, string name)
        {
            if (!_scenePromiseContainers.TryGetValue(name, out var assetPromise))
            {
                var scene = SceneManager.GetSceneByName(SceneName);
                AssertUtil.NotNull(scene, SceneExceptionType.create_from_invalidScene);

                assetPromise = _assetLoader.Instantiate(assetKey);
                _scenePromiseContainers.Add(name, assetPromise);
                GameObject go = assetPromise.GetResult<GameObject>();
                go.name = name;
                SceneManager.MoveGameObjectToScene(go, scene);
            }

            return assetPromise.GetResult<GameObject>();
        }

        public void ReturnInstance(string name)
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                GameObject.Destroy(instance);
            }
            if (!_scenePromiseContainers.TryGetValue(name, out var assetPromise))
            {
                _assetLoader.ReleaseInstance(assetPromise);
            }
        }

        public void Dispose()
        {
            foreach (var item in _sceneInstanceContainers)
            {
                GameObject.Destroy(item.Value);
            }

            foreach (var item in _scenePromiseContainers)
            {
                _assetLoader.ReleaseInstance(item.Value);
            }
            _sceneInstanceContainers.Clear();
            SceneName = string.Empty;
        }


    }
}