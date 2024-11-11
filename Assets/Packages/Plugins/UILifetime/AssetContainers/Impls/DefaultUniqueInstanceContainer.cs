using System;
using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.LifeTime
{
    public  class DefaultUniqueInstanceContainer : BaseInstanceCreator, IUniqueInstanceContainer
    {
        [Inject] private IAssetLoader _assetLoader;
        // TODO delete
        // PLAN since addressable has its reference count

        private class InstanceWrapper : IDisposable, IPoolNode<InstanceWrapper>
        {
            private static ReusablePool<InstanceWrapper> _pool;

            private Object _instance;
            private int _count;
            private InstanceWrapper _nextNode;


            public ref InstanceWrapper NextNode => ref _nextNode;
            public bool IsRecycled { get; set; }


            public static InstanceWrapper Create(Object gameObject)
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

        private readonly Dictionary<IAssetKey, InstanceWrapper> _instancePromises = new();

        internal DefaultUniqueInstanceContainer(string sceneName):base(sceneName)
        {
            
        }
        public async PromiseTask<T> Instantiate<T>(IAssetKey assetKey) where T : Object
        {
            if (!_instancePromises.TryGetValue(assetKey, out var instanceWrapper))
            {
                var asset = await _assetLoader.Load<T>(assetKey);
                InstantiateAssets(asset, assetKey.Key, 1, assetKey);
            }

            return instanceWrapper?.GetResult<T>();
        }

        public async PromiseTask<T> InstantiateAsync<T>(IAssetKey assetKey, UnsafeCancellationToken cancellation) where T : Object
        {
            if (!_instancePromises.TryGetValue(assetKey, out var instanceWrapper))
            {
                var asset = await _assetLoader.LoadAsync<T>(assetKey, cancellation);
                await InstantiateAssetsAsync(asset, assetKey.Key, 1, cancellation,assetKey);
            }

            return instanceWrapper?.GetResult<T>();
        }

        public T GetInstance<T>(IAssetKey assetKey) where T : Object
        {
            if (_instancePromises.TryGetValue(assetKey, value: out var promise))
            {
                return promise.GetResult<T>();
            }
            return null;
        }

        public async PromiseTask Unload(IAssetKey assetKey)
        {
            if (_instancePromises.TryGetValue(assetKey, out var promise))
            {
                promise.TryDestroy();
                await _assetLoader.Unload(assetKey);
            }
        }

        public async PromiseTask UnloadAll()
        {
            foreach (var item in _instancePromises)
            {
                item.Value.TryDestroy();
                await _assetLoader.Unload(item.Key);
            }
        }

        public virtual void Dispose()
        {
            foreach (var item in _instancePromises)
            {
                item.Value.Dispose();
            }
            _instancePromises.Clear();
        }

        protected sealed override void OnCreate(IAssetKey assetKey, Object instance)
        {
            OnInstantiate(instance);
            _instancePromises.Add(assetKey, InstanceWrapper.Create(instance));
        }
        
    }
}
