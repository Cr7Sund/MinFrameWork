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

        public async PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey, UnsafeCancellationToken cancellation) where T : Object
        {
            if (!_containers.ContainsKey(assetKey))
            {
                var asset = await Loader.LoadAsync<T>(assetKey, cancellation);
                if (!_containers.ContainsKey(assetKey))
                {
                    _containers.Add(assetKey, asset);
                }
            }

            return _containers[assetKey] as T;
        }

        public async PromiseTask<Object[]> LoadGroup(IEnumerable<IAssetKey> assetKeys, UnsafeCancellationToken cancellation)
        {
            List<PromiseTask<Object>> groups = new List<PromiseTask<Object>>();
            foreach (var assetKey in assetKeys)
            {
                if (!_containers.ContainsKey(assetKey))
                {
                    groups.Add(Loader.LoadAsync<Object>(assetKey, cancellation));
                }
            }

            return await PromiseTask<Object>.WhenAll(groups);
        }

        public async PromiseTask<T[]> LoadGroup<T>(IEnumerable<IAssetKey> assetKeys, UnsafeCancellationToken cancellation) where T : Object
        {
            var groups = new List<PromiseTask<T>>();
            foreach (var assetKey in assetKeys)
            {
                if (!_containers.ContainsKey(assetKey))
                {
                    groups.Add(Loader.LoadAsync<T>(assetKey, cancellation));
                }
            }

            return await PromiseTask<T>.WhenAll(groups);
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

        public async PromiseTask<T[]> ParallelLoadAssets<T>(IEnumerable<IAssetKey> assetKeys, IList<IAssetKey> inFilterKeys, IList<PromiseTask<T>> inFilterTasks, UnsafeCancellationToken cancellation) where T : Object
        {
            foreach (var assetKey in assetKeys)
            {
                if (!_containers.ContainsKey(assetKey))
                {
                    inFilterKeys.Add(assetKey);
                    inFilterTasks.Add(Loader.LoadAsync<T>(assetKey, cancellation));
                }
            }

            var result = await PromiseTask<T>.WhenAll(inFilterTasks);
            for (int i = 0; i < inFilterKeys.Count; i++)
            {
                _containers.Add(inFilterKeys[i], result[i]);
            }
            return result;
        }

        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_containers.Count, 0);
        }

    }
}