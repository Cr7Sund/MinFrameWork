using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Server.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseUniqueInstanceContainer : BaseAssetContainer, IAssetInstanceContainer
    {
        private Dictionary<string, IAssetPromise> _instancePromises = new();


        public IAssetPromise CreateInstance(IAssetKey assetKey)
        {
            if (_instancePromises.ContainsKey(assetKey.Key))
            {
                return _instancePromises[assetKey.Key];
            }

            var instancePromise = new AssetPromise();
            _instancePromises.Add(assetKey.Key, instancePromise);

            LoadAsset(assetKey).Then((asset) =>
            {
                var instance = InstantiateAsset(asset);
                instancePromise.Resolve(instance);
            });


            return instancePromise;
        }

        public IAssetPromise CreateInstanceAsync(IAssetKey assetKey)
        {
            if (_instancePromises.ContainsKey(assetKey.Key))
            {
                return _instancePromises[assetKey.Key];
            }

            var instancePromise = new AssetPromise();
            _instancePromises.Add(assetKey.Key, instancePromise);

            LoadAssetAsync(assetKey).Then((asset) =>
            {
                var instance = InstantiateAsset(asset);
                instancePromise.Resolve(instance);
            });


            return instancePromise;
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
                item.Value.Dispose(); // assetPromise will handle GameObject Destroy
            }

            base.Dispose();
            _instancePromises.Clear();
        }

        private T InstantiateAsset<T>(T asset) where T : Object
        {
            var instance = GameObject.Instantiate(asset);
            // OnInstantiate(instance);

            return instance;
        }

    }
}