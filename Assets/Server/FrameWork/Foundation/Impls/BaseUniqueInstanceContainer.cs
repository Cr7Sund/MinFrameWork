using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseUniqueInstanceContainer : BaseAssetContainer, IUniqueInstanceContainer
    {
        private Dictionary<IAssetKey, GameObject> _instancePromises = new();


        public async PromiseTask<T> CreateInstance<T>(IAssetKey assetKey) where T : Object
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                return _instancePromises[assetKey] as T;
            }

            var asset = await LoadAsset<T>(assetKey);
            var instance = InstantiateAsset(asset);
            _instancePromises.Add(assetKey, instance as GameObject);
            return instance;
        }

        public async PromiseTask<T> CreateInstanceAsync<T>(IAssetKey assetKey) where T : Object
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                return _instancePromises[assetKey] as T;
            }

            var asset = await LoadAssetAsync<T>(assetKey);
            var instance = InstantiateAsset(asset);
            _instancePromises.Add(assetKey, instance as GameObject);
            return instance;
        }

        public override void Unload(IAssetKey assetKey)
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                GameObject.Destroy(_instancePromises[assetKey]);
                base.Unload(assetKey);
                _instancePromises.Remove(assetKey);
            }
        }

        public override void UnloadAll()
        {
            foreach (var item in _instancePromises)
            {
                GameObject.Destroy(item.Value);
                base.Unload(item.Key);
            }
            _instancePromises.Clear();
        }

        public override void Dispose()
        {
            AssertUtil.LessOrEqual(_instancePromises.Count, 0);
        }

        private T InstantiateAsset<T>(T asset) where T : Object
        {
            var instance = GameObject.Instantiate(asset);
            // OnInstantiate(instance);

            return instance;
        }
    }
}
