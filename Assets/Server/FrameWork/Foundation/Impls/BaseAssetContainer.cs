using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseAssetContainer : IAssetContainer
    {
        private Dictionary<IAssetKey, Object> _containers = new();

        protected abstract IAssetLoader Loader { get; }

        public async PromiseTask<T> LoadAsset<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey))
            {
                var asset = await Loader.Load<T>(assetKey);
                _containers.Add(assetKey, asset);
            }

            return _containers[assetKey] as T;
        }

        public async PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey) where T : Object
        {
            if (!_containers.ContainsKey(assetKey))
            {
                var asset = await Loader.LoadAsync<T>(assetKey);
                _containers.Add(assetKey, asset);
            }

            return _containers[assetKey] as T;
        }

        public void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            if (_containers.ContainsKey(assetKey))
            {
                Loader.RegisterCancelLoad(assetKey, cancellation);
                _containers.Remove(assetKey);
            }
        }

        public virtual void Unload(IAssetKey key)
        {
            if (_containers.ContainsKey(key))
            {
                Loader.Unload(key);
                _containers.Remove(key);
            }
        }

        public virtual void UnloadAll()
        {
            foreach (var item in _containers)
            {
                Loader.Unload(item.Key);
            }
            _containers.Clear();
        }

        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_containers.Count, 0);
        }

    }
}