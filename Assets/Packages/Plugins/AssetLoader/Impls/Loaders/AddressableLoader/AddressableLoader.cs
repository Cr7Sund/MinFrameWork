using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AddressableLoader : IAssetLoader
    {
        private Dictionary<string, IAssetPromise> _controlIdToHandles = new();
        private int _nextControlId;

        public bool IsInit { get; private set; }


        public async PromiseTask Init()
        {
            var handler = Addressables.InitializeAsync();
            await handler.Task;
            IsInit = true;
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
                        sb.Append(item.Value.Key);
                        sb.Append(", ");
                    }
                    Console.Warn("List Below {Msg}", sb.ToString());
                }
            }

            foreach (IAssetPromise assetPromise in _controlIdToHandles.Values)
            {
                Addressables.Release(assetPromise.Handler);
            }

            _nextControlId = 0;
            _controlIdToHandles.Clear();
        }

        public void Update()
        {
            foreach (var item in _controlIdToHandles)
            {
                var dl = item.Value.Handler;
                while (dl.PercentComplete < 1 && !dl.IsDone)
                {
                    item.Value.ReportProgress(dl.PercentComplete);
                }
            }
        }

        public PromiseTask<T> Load<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key.Key, false);
        }

        public PromiseTask<T> LoadAsync<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key.Key, true);
        }

        [Obsolete]
        public void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            var key = assetKey.Key;
            if (!_controlIdToHandles.ContainsKey(key))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{key}).");
            }

            var handler = _controlIdToHandles[key];
            cancellation.Register(() =>
            {
                Unload(assetKey);
                handler.Cancel();
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

            Addressables.Release(_controlIdToHandles[key].Handler);
            _controlIdToHandles.Remove(key);
        }

        private async PromiseTask<T> LoadInternal<T>(string key, bool isAsync) where T : Object
        {
            var controlId = _nextControlId++;


            if (_controlIdToHandles.TryGetValue(key, out var setter))
            {
                await setter.AsNewTask();
                return setter.GetResult<T>();
            }

            try
            {
                var addressableHandle = Addressables.LoadAssetAsync<T>(key);
                //PLAN create with pool
                setter = new AssetPromise(addressableHandle, key, controlId);
                _controlIdToHandles.Add(key, setter);

                if (!isAsync) addressableHandle.WaitForCompletion();

                var asset = await addressableHandle.Task;
                setter.Resolve(asset);
                return asset;
            }
            catch (Exception e)
            {
                if (setter != null)
                {
                    setter = new AssetPromise(key, controlId);
                }
                setter.Reject(e);
                throw;
            }

        }
    }
}
