using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Impl;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cr7Sund.AssetLoader.Impl
{
    // 1. the asset should be load unique
    // 2. we to cache the AsyncOperationHandle (since it's struct will be copy each time)
    public class AddressableLoader : IAssetLoader
    {

        private Dictionary<IAssetKey, AsyncChainOperations> _controlIdToHandles = new();

        public bool IsInit { get; private set; }


        public async PromiseTask Init()
        {
            var handler = Addressables.InitializeAsync();
            await handler.Task;
            IsInit = true;
        }


        public PromiseTask<T> Load<T>(IAssetKey key)
        {
            return LoadInternal<T>(key, false, default);
        }

        public PromiseTask<T> LoadAsync<T>(IAssetKey key, CancellationToken cancellation)
        {
            return LoadInternal<T>(key, true, cancellation);
        }

        public async PromiseTask RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            var key = assetKey.Key;
            if (!_controlIdToHandles.ContainsKey(assetKey))
            {
                throw new MyException(
                    $"There is no asset that has been requested to cancel (Asset:{key}).");
            }

            var chainOperation = _controlIdToHandles[assetKey];

            await chainOperation.Chain();
            //PLAN : replace with cancel 
            await Unload(assetKey);
        }

        public async PromiseTask Unload(IAssetKey assetKey)
        {
            var key = assetKey.Key;
            if (!_controlIdToHandles.ContainsKey(assetKey))
            {
                return;
                // throw new MyException(
                //     $"There is no asset that has been requested for release (Asset:{key}).");
            }

            await _controlIdToHandles[assetKey].Chain();
            _controlIdToHandles[assetKey].Unload(OnUnloadHandle);
            _controlIdToHandles.Remove(assetKey);
        }

        private static void OnUnloadHandle(AsyncChainOperations asyncChain)
        {
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

            if (_controlIdToHandles.Count > 0)
            {
                Console.Warn("still exist {Count} left", _controlIdToHandles.Count);
                if (MacroDefine.IsEditor)
                {
                    var sb = new StringBuilder();
                    foreach (var item in _controlIdToHandles)
                    {
                        sb.Append(item.Key);
                        sb.Append(", ");
                    }
                    Console.Warn("List Below {Msg}", sb.ToString());
                }
            }

            _controlIdToHandles.Clear();
        }

        private async PromiseTask<T> LoadInternal<T>(IAssetKey assetKey, bool isAsync, CancellationToken cancellation)
        {
            string key = assetKey.Key;
            if (_controlIdToHandles.TryGetValue(assetKey, out var chainOperation))
            {
                await chainOperation.Chain();
                return chainOperation.GetResult<T>();
            }

            AsyncOperationHandle handler = Addressables.LoadAssetAsync<T>(key);
            if (!isAsync) handler.WaitForCompletion();
            chainOperation = AsyncChainOperations.Start(ref handler, cancellation);

            _controlIdToHandles.Add(assetKey, chainOperation);

            // equal to below
            // await addressableHandle.Task;
            await chainOperation.Chain();
            return chainOperation.GetResult<T>();
        }
    }
}
