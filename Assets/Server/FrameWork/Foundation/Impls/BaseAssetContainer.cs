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

        public async PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey, CancellationToken cancellation) where T : Object
        {
            if (!_containers.ContainsKey(assetKey))
            {
                var asset = await Loader.LoadAsync<T>(assetKey, cancellation);
                _containers.Add(assetKey, asset);
            }

            return _containers[assetKey] as T;
        }

        public async PromiseTask CancelLoadAsync(IAssetKey assetKey, CancellationToken cancellation)
        {
            await Loader.RegisterCancelLoad(assetKey, cancellation);
            AssertUtil.IsFalse(_containers.ContainsKey(assetKey));
        }

        public virtual async PromiseTask Unload(IAssetKey key)
        {
            if (_containers.ContainsKey(key))
            {
                await Loader.Unload(key);
                _containers.Remove(key);
            }
        }

        public virtual async PromiseTask UnloadAll()
        {
            foreach (var item in _containers)
            {
                await Loader.Unload(item.Key);
            }
            _containers.Clear();
        }

        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_containers.Count, 0);
        }

    }
}