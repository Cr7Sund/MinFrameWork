using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseUniqueInstanceContainer : BaseAssetContainer, IUniqueInstanceContainer
    {
        private class InstanceWrapper : IDisposable, IPoolNode<InstanceWrapper>
        {
            private static ReusablePool<InstanceWrapper> _pool = new ReusablePool<InstanceWrapper>();

            private GameObject _instance;
            private int _count;
            private InstanceWrapper _nextNode;


            public ref InstanceWrapper NextNode => ref _nextNode;
            public bool IsRecycled { get; set; }


            public static InstanceWrapper Create(GameObject gameObject)
            {
                if (!_pool.TryPop(out var wrapper))
                {
                    wrapper = new InstanceWrapper();
                }

                wrapper._instance = gameObject;
                wrapper._count++;
                return wrapper;
            }

            public T GetResult<T>() where T : Object
            {
                T result = _instance as T;
                _count++;
                return result;
            }

            public bool TryDestroy()
            {
                _count--;
                if (_count == 0)
                {
                    GameObjectUtil.Destroy(_instance);
                    Dispose();

                    _pool.TryPush(this);
                }
                return _count == 0;
            }

            public void Dispose()
            {
                AssertUtil.AreEqual(_count, 0);
                _instance = default;
            }
        }

        private Dictionary<IAssetKey, InstanceWrapper> _instancePromises = new();


        public async PromiseTask<T> CreateInstance<T>(IAssetKey assetKey) where T : Object
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                return _instancePromises[assetKey].GetResult<T>();
            }

            var asset = await LoadAsset<T>(assetKey);
            var instance = InstantiateAsset(asset);
            _instancePromises.Add(assetKey, InstanceWrapper.Create(instance as GameObject));
            return instance;
        }

        public async PromiseTask<T> CreateInstanceAsync<T>(IAssetKey assetKey, UnsafeCancellationToken cancellation) where T : Object
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                return _instancePromises[assetKey].GetResult<T>();
            }

            var asset = await LoadAssetAsync<T>(assetKey, cancellation);
            var instance = InstantiateAsset(asset);
            _instancePromises.Add(assetKey, InstanceWrapper.Create(instance as GameObject));
            return instance;
        }

        public override async PromiseTask Unload(IAssetKey assetKey)
        {
            if (_instancePromises.ContainsKey(assetKey))
            {
                _instancePromises[assetKey].TryDestroy();
                await base.Unload(assetKey);
            }
        }

        public override async PromiseTask UnloadAll()
        {
            foreach (var item in _instancePromises)
            {
                item.Value.TryDestroy();
                await base.Unload(item.Key);
            }
        }

        public override void Dispose()
        {
            foreach (var item in _instancePromises)
            {
                item.Value.Dispose();
            }
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
