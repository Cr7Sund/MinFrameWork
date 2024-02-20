using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AddressableLoader : IAssetLoader
    {
        private Dictionary<int, IAssetPromise> _controlIdToHandles;
        private int _nextControlId;

        public bool IsInit { get; private set; }

        public void Init()
        {
            _controlIdToHandles =
                new Dictionary<int, IAssetPromise>();

            Addressables.InitializeAsync();

            IsInit = true;
        }
        public void Dispose()
        {
            IsInit = false;

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

        public T Load<T>(IAssetKey key) where T : Object
        {
            var loadPromise = LoadInternal<T>(key, false);
            return loadPromise.Handler.Result as T;
        }
        public IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key, true);
        }
        public void Unload<T>(IAssetPromise assetPromise) where T : Object
        {
            if (!_controlIdToHandles.ContainsKey(assetPromise.ControlId))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{assetPromise.Key} ControlId: {assetPromise.ControlId}).");
            }

            var addressableHandle = _controlIdToHandles[assetPromise.ControlId];
            _controlIdToHandles.Remove(assetPromise.ControlId);
            Addressables.Release(addressableHandle);
        }
        public IAssetPromise InstantiateAsync(IAssetKey key)
        {
            return InstantiateInternal(key, true);
        }
        public IAssetPromise Instantiate(IAssetKey key)
        {
            return InstantiateInternal(key, false);
        }

        public void ReleaseInstance(IAssetPromise assetPromise)
        {
            if (!_controlIdToHandles.ContainsKey(assetPromise.ControlId))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{assetPromise.Key} ControlId: {assetPromise.ControlId}).");
            }

            var addressableHandle = _controlIdToHandles[assetPromise.ControlId];
            _controlIdToHandles.Remove(assetPromise.ControlId);
            Addressables.ReleaseInstance(addressableHandle.Handler.Result as GameObject);
        }

        private IAssetPromise LoadInternal<T>(IAssetKey key, bool isAsync) where T : Object
        {
            var addressableHandle = Addressables.LoadAssetAsync<T>(key.Key);
            if (!isAsync) addressableHandle.WaitForCompletion();

            var controlId = _nextControlId++;
            var setter = new AssetPromise(addressableHandle, key, controlId);
            _controlIdToHandles.Add(controlId, setter);

            addressableHandle.ToPromise(setter);

            return setter;
        }
        private IAssetPromise InstantiateInternal(IAssetKey key, bool isAsync)
        {
            AsyncOperationHandle<GameObject> addressableHandle = Addressables.InstantiateAsync(key.Key);
            if (!isAsync) addressableHandle.WaitForCompletion();

            var controlId = _nextControlId++;
            var setter = new AssetPromise(addressableHandle, key, controlId);
            _controlIdToHandles.Add(controlId, setter);

            addressableHandle.ToPromise(setter);
            return setter;
        }

    }
}
