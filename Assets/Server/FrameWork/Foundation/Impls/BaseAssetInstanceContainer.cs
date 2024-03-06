using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Server.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseAssetInstanceContainer : BaseAssetContainer, IAssetInstanceContainer
    {
        private Dictionary<string, IAssetPromise> _instancePromises = new();


        public IAssetPromise CreateInstance<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey.Key))
            {
                var promise = Loader.Instantiate(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise;
            }
            else
            {
                if (_containers[assetKey.Key].IsInstantiate)
                {
                    return _containers[assetKey.Key];
                }
                else
                {
                    if (_instancePromises.ContainsKey(assetKey.Key))
                    {
                        return _instancePromises[assetKey.Key];
                    }
                    else
                    {
                        T asset = _containers[assetKey.Key].GetResultSync<T>();  // resolved
                        var instance = InstantiateAsset(asset);

                        var instancePromise = new AssetPromise();
                        instancePromise.Resolve(instance);
                        _instancePromises.Add(assetKey.Key, instancePromise);

                        return instancePromise;
                    }
                }
            }
        }

        public T CreateInstanceSync<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey.Key))
            {
                var promise = Loader.Instantiate(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise.GetResult<T>();
            }
            else
            {
                if (_containers[assetKey.Key].IsInstantiate)
                {
                    return _containers[assetKey.Key].GetResultSync<T>();
                }
                else
                {
                    if (_instancePromises.ContainsKey(assetKey.Key))
                    {
                        return _instancePromises[assetKey.Key].GetResult<T>();
                    }
                    else
                    {
                        T asset = _containers[assetKey.Key].GetResultSync<T>();  // resolved
                        var instance = InstantiateAsset(asset);

                        var instancePromise = new AssetPromise();
                        instancePromise.Resolve(instance);
                        _instancePromises.Add(assetKey.Key, instancePromise);

                        return instance;
                    }
                }
            }
        }

        public IAssetPromise CreateInstanceAsync<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey.Key))
            {
                var promise = Loader.InstantiateAsync(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise;
            }
            else
            {
                if (_containers[assetKey.Key].IsInstantiate)
                {
                    return _containers[assetKey.Key];
                }
                else
                {
                    if (_instancePromises.ContainsKey(assetKey.Key))
                    {
                        return _instancePromises[assetKey.Key];
                    }
                    else
                    {
                        // asset loading -> instantiate
                        // 1. preload
                        // 2. instantiate externally -> instantiate internally * forbid
                        _containers[assetKey.Key].Cancel();

                        var instancePromise = new AssetPromise();
                        _containers[assetKey.Key].Then((asset) =>
                        {
                            var instance = InstantiateAsset(asset);
                            instancePromise.Resolve(instance);
                        });
                        _instancePromises.Add(assetKey.Key, instancePromise);

                        return instancePromise;
                    }
                }
            }
        }

        public bool TryGetInstance<T>(IAssetKey key, out T result) where T : Object
        {
            result = null;
            if (_containers.TryGetValue(key.Key, out var assetPromise)
                    && assetPromise.IsInstantiate)
            {
                result = assetPromise.GetResult<T>();
                return true;
            }
            if (_instancePromises.ContainsKey(key.Key))
            {
                result = _instancePromises[key.Key].GetResult<T>();
                return true;
            }

            return false;
        }

        public override void Unload(IAssetKey key)
        {
            if (_instancePromises.ContainsKey(key.Key))
            {
                _instancePromises[key.Key].Dispose();
                _instancePromises.Remove(key.Key);
            }
            base.Unload(key);
        }

        public override void Dispose()
        {
            foreach (var item in _instancePromises)
            {
                item.Value.Dispose();
            }

            base.Dispose();
            _instancePromises.Clear();
        }
        private Object InstantiateAsset(Object asset)
        {
            var instance = GameObject.Instantiate(asset);
            // OnInstantiate(instance);

            return instance;
        }

        private T InstantiateAsset<T>(T asset) where T : Object
        {
            var instance = GameObject.Instantiate(asset);
            // OnInstantiate(instance);

            return instance;
        }

    }
}