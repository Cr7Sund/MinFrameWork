using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Server.Apis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class InstanceContainer : IInstanceContainer
    {
        protected abstract IAssetLoader _loader { get; }

        private Dictionary<string, GameObject> _sceneInstanceContainers = new();
        private Dictionary<string, IAssetPromise> _scenePromiseContainers = new();



        public GameObject CreateInstance(string name, params Type[] components)
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, components);
                MoveInstanceToScene(instance);
                _sceneInstanceContainers.Add(name, instance);
            }

            return instance;
        }

        public T CreateInstanceWithComponent<T>(string name) where T : Object
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, typeof(T));
                MoveInstanceToScene(instance);
                _sceneInstanceContainers.Add(name, instance);
            }

            return instance.GetComponent<T>();
        }

        public GameObject CreateInstance(string name, IAssetPromise assetPromise)
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                instance = GameObject.Instantiate(assetPromise.GetResult<GameObject>());
                instance.name = name;
                MoveInstanceToScene(instance);
                _sceneInstanceContainers.Add(name, instance);
            }

            return instance;
        }

        public GameObject LoadInstance(IAssetKey assetKey, string name)
        {
            if (!_sceneInstanceContainers.ContainsKey(name))
            {
                if (!_scenePromiseContainers.TryGetValue(name, out var assetPromise))
                {
                    assetPromise = _loader.Instantiate(assetKey);
                    _scenePromiseContainers.Add(name, assetPromise);
                }

                GameObject instance = assetPromise.GetResultSync<GameObject>();
                instance.name = name;
                MoveInstanceToScene(instance);
                _sceneInstanceContainers.Add(name, instance);
            }

            return _sceneInstanceContainers[name];
        }

        public IAssetPromise LoadInstanceAsync(IAssetKey assetKey, string name)
        {
            if (_sceneInstanceContainers.ContainsKey(name))
            {
                var resultPromise = new AssetPromise();
                resultPromise.Resolve(_sceneInstanceContainers[name]);
                return resultPromise;
            }
            else
            {
                if (_scenePromiseContainers.TryGetValue(name, out var assetPromise))
                {
                    return assetPromise;
                }
                else
                {
                    assetPromise = _loader.InstantiateAsync(assetKey);

                    var resultPromise = new AssetPromise();
                    assetPromise.Then(asset =>
                             {
                                 var instance = asset as GameObject;
                                 instance.name = name;
                                 MoveInstanceToScene(instance);
                                 _sceneInstanceContainers.Add(name, instance);
                                 resultPromise.Resolve(instance);
                             });

                    _scenePromiseContainers.Add(name, resultPromise);
                    return resultPromise;
                }
            }

        }

        public void ReturnInstance(string name)
        {
            if (!_sceneInstanceContainers.TryGetValue(name, out var instance))
            {
                GameObject.Destroy(instance);
            }
            if (!_scenePromiseContainers.TryGetValue(name, out var assetPromise))
            {
                _loader.Unload(assetPromise);
            }
        }

        protected abstract void MoveInstanceToScene(GameObject instance);


        public virtual void Dispose()
        {
            foreach (var item in _sceneInstanceContainers)
            {
                GameObject.Destroy(item.Value);
            }

            foreach (var item in _scenePromiseContainers)
            {
                _loader.Unload(item.Value);
            }
            _sceneInstanceContainers.Clear();
        }

    }
}