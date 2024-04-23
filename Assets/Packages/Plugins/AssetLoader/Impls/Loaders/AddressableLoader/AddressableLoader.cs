using System;
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
        private Dictionary<string, AsynChainOperations> _controlIdToHandles = new();

        public bool IsInit { get; private set; }


        public async PromiseTask Init()
        {
            var handler = Addressables.InitializeAsync();
            await handler.Task;
            IsInit = true;
        }


        public PromiseTask<T> Load<T>(IAssetKey key)
        {
            return LoadInternal<T>(key.Key, false);
        }

        public PromiseTask<T> LoadAsync<T>(IAssetKey key)
        {
            return LoadInternal<T>(key.Key, true);
        }

        public void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            throw new NotImplementedException();

            var key = assetKey.Key;
            if (!_controlIdToHandles.ContainsKey(key))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{key}).");
            }

            var handler = _controlIdToHandles[key];
            cancellation.Register(() =>
            {
                // Unload(assetKey);
                handler.Cancel(cancellation);
            });

        }

        public void Unload(IAssetKey assetKey)
        {
            var key = assetKey.Key;
            if (!_controlIdToHandles.ContainsKey(key))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{key}).");
            }

            _controlIdToHandles[key].Unload(OnUnloadHandle);
            _controlIdToHandles.Remove(key);
        }

        private static void OnUnloadHandle(AsynChainOperations asynChain)
        {
            Addressables.Release(asynChain.Handler);
            asynChain.TryReturn();
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

        private async PromiseTask<T> LoadInternal<T>(string key, bool isAsync)
        {
            if (_controlIdToHandles.TryGetValue(key, out var setter))
            {
                await setter.Chain();
                return setter.GetResult<T>();
            }

            AsyncOperationHandle handler = Addressables.LoadAssetAsync<T>(key);
            if (!isAsync) handler.WaitForCompletion();
            setter = AsynChainOperations.Start(ref handler);
            _controlIdToHandles.Add(key, setter);

            // equal to below
            // await addressableHandle.Task;
            await setter.Chain();
            return setter.GetResult<T>();
        }
    }
}
