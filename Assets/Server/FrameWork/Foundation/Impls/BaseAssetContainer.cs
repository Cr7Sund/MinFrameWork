using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseAssetContainer : IAssetContainer
    {
        protected Dictionary<string, IAssetPromise> _containers = new();

        protected abstract IAssetLoader Loader { get; }


        public IAssetPromise GetAsset<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey.Key))
            {
                var promise = Loader.Load<T>(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise;
            }
            else
            {
                _containers[assetKey.Key].GetResultSync<T>();

                return _containers[assetKey.Key];
            }
        }

        public IAssetPromise GetAssetAsync(IAssetKey assetKey)
        {
            if (_containers.ContainsKey(assetKey.Key))
            {
                return _containers[assetKey.Key];
            }
            else
            {
                var promise = Loader.LoadAsync<Object>(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise;
            }
        }

        public T GetAssetSync<T>(IAssetKey assetKey) where T : Object
        {
            if (_containers.ContainsKey(assetKey.Key))
            {
                return _containers[assetKey.Key].GetResultSync<T>();
            }
            else
            {
                var promise = Loader.Load<Object>(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise.GetResultSync<T>();
            }
        }

        public virtual void Unload(IAssetKey key)
        {
            if (_containers.ContainsKey(key.Key))
            {
                Loader.Unload(_containers[key.Key]);
                _containers.Remove(key.Key);
            }
        }

        public bool ContainsAsset(IAssetKey key) => _containers.ContainsKey(key.Key);

        public virtual void Dispose()
        {
            foreach (var item in _containers)
            {
                Loader.Unload(item.Value);
            }

            _containers.Clear();
        }

    }
}