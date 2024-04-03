using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseAssetContainer : IAssetContainer
    {
        private Dictionary<IAssetKey, IAssetPromise> _containers = new();

        protected abstract IAssetLoader Loader { get; }


        public IAssetPromise LoadAsset(IAssetKey assetKey)
        {
            if (_containers.ContainsKey(assetKey))
            {
                _containers[assetKey].ForceGetResult<Object>();
                return _containers[assetKey];
            }
            else
            {
                var promise = Loader.Load<Object>(assetKey);
                _containers.Add(assetKey, promise);
                return promise;
            }
        }

        public IAssetPromise LoadAssetAsync(IAssetKey assetKey)
        {
            if (_containers.ContainsKey(assetKey))
            {
                return _containers[assetKey];
            }
            else
            {
                var promise = Loader.LoadAsync<Object>(assetKey);
                _containers.Add(assetKey, promise);
                return promise;
            }
        }

        [Obsolete]
        public T GetAssetSync<T>(IAssetKey assetKey) where T : Object
        {
            if (_containers.ContainsKey(assetKey))
            {
                return _containers[assetKey].ForceGetResult<T>();
            }
            else
            {
                var promise = Loader.Load<T>(assetKey);
                _containers.Add(assetKey, promise);
                return promise.ForceGetResult<T>();
            }
        }

        public virtual void Unload(IAssetKey key)
        {
            if (_containers.ContainsKey(key))
            {
                Loader.Unload(_containers[key]);
                _containers.Remove(key);
            }
        }

        public bool ContainsAsset(IAssetKey key) => _containers.ContainsKey(key);

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