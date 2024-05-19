using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{
    // 1. the asset should be load unique
    // 2. we to cache the AsyncOperationHandle (since it's struct will be copy each time)
    public class AddressableLoader : IAssetLoader
    {
        private Dictionary<IAssetKey, AsyncChainOperations<Object>> _assetKeyToHandles = new();
        private readonly Action<AsyncChainOperations<Object>> _onUnloadedAction = OnUnloadHandle;

        public bool IsInit { get; private set; }


        public async PromiseTask Init()
        {
            var handler = Addressables.InitializeAsync();
            await handler.Task;
            IsInit = true;
        }

        public PromiseTask<T> Load<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key, false, default);
        }

        public PromiseTask<T> LoadAsync<T>(IAssetKey key, UnsafeCancellationToken cancellation) where T : Object
        {
            return LoadInternal<T>(key, true, cancellation);
        }

        public async PromiseTask Unload(IAssetKey assetKey)
        {
            var key = assetKey.Key;
            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                return;
                // throw new MyException(
                //     $"There is no asset that has been requested for release (Asset:{key}).");
            }

            var asyncChainOperations = _assetKeyToHandles[assetKey];
            // we need to remove the cache first
            // in case when we start loading during loading
            // we want not to unload the asset which will be happened in reference count is zero
            // so we need to start new loading operation to increase count first 
            // and we can't await unloading finish which will decrease reference count to zero
            _assetKeyToHandles.Remove(assetKey);
            await asyncChainOperations.Chain();
            asyncChainOperations.RegisterUnload(_onUnloadedAction);
        }

        private static void OnUnloadHandle(AsyncChainOperations<Object> asyncChain)
        {
            // if (!asyncChain.Handler.IsValid())
            // {
            // }
            Addressables.Release(asyncChain.Handler);
            asyncChain.TryReturn();
        }

        public PromiseTask DestroyAsync()
        {
            Dispose();
            return PromiseTask.CompletedTask;
        }

        public void Dispose()
        {
            IsInit = false;

            if (_assetKeyToHandles.Count > 0)
            {
                Console.Warn("still exist {Count} left", _assetKeyToHandles.Count);
                if (MacroDefine.IsEditor)
                {
                    var sb = new StringBuilder();
                    foreach (var item in _assetKeyToHandles)
                    {
                        sb.Append(item.Key.Key);
                        sb.Append(", ");
                    }
                    Console.Warn("List Below {Msg}", sb.ToString());
                }
            }

            _assetKeyToHandles.Clear();
        }

        private async PromiseTask<T> LoadInternal<T>(IAssetKey assetKey, bool isAsync, UnsafeCancellationToken cancellation) where T : Object
        {
            string key = assetKey.Key;
            if (_assetKeyToHandles.TryGetValue(assetKey, out var chainOperation))
            {
                await chainOperation.Chain();
                return chainOperation.GetResult<T>();
            }

            var handler = Addressables.LoadAssetAsync<Object>(key);
            if (!isAsync)
            {
                handler.WaitForCompletion();
            }

            chainOperation = AsyncChainOperations<Object>.Start(ref handler, assetKey.Key, cancellation);
            _assetKeyToHandles.Add(assetKey, chainOperation);
            RegisterCancel(assetKey, cancellation);

            // equal to below
            // await addressableHandle.Task;
            await chainOperation.Chain();
            return chainOperation.GetResult<T>();
        }

        private void RegisterCancel(IAssetKey assetKey, UnsafeCancellationToken cancellation)
        {
            if (!cancellation.IsValid)
            {
                return;
            }
            if (cancellation.IsCancellationRequested)
            {
                OnCancel(assetKey, cancellation);
            }
            else
            {
                cancellation.Register(() => OnCancel(assetKey, cancellation));
            }
        }

        private void OnCancel(IAssetKey assetKey, UnsafeCancellationToken cancellation)
        {
            // unload win
            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                return;
            }

            AsyncChainOperations<Object> chainOperation = _assetKeyToHandles[assetKey];
            chainOperation.RegisterCancel(assetKey.Key, cancellation);
            chainOperation.RegisterUnload(_onUnloadedAction);
            _assetKeyToHandles.Remove(assetKey);
        }
    }
}
