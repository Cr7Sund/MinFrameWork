using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<int, IAssetPromise> _controlIdToHandles = new();
        private int _nextControlId;

        public bool IsInit { get; private set; }


        public void Init()
        {
            Addressables.InitializeAsync();

            IsInit = true;
        }
        public void Dispose()
        {
            IsInit = false;

            var promiseList = _controlIdToHandles.Values.ToArray();
            for (int i = promiseList.Length - 1; i >= 0; i--)
            {
                IAssetPromise value = promiseList[i];
                UnloadAssetPromise(value);
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

        public IAssetPromise Load<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key.Key, false);
        }
      
        public IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key.Key, true);
        }
      
        public void Unload<T>(IAssetPromise assetPromise) where T : Object
        {
            if (!_controlIdToHandles.ContainsKey(assetPromise.ControlId))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{assetPromise.Key} ControlId: {assetPromise.ControlId}).");
            }

            _controlIdToHandles.Remove(assetPromise.ControlId);
            Addressables.Release(assetPromise.Handler);
        }
      
        public IAssetPromise InstantiateAsync(IAssetKey key)
        {
            return InstantiateInternal(key.Key, true);
        }
      
        public IAssetPromise Instantiate(IAssetKey key)
        {
            return InstantiateInternal(key.Key, false);
        }

        public void ReleaseInstance(IAssetPromise assetPromise)
        {
            if (!_controlIdToHandles.ContainsKey(assetPromise.ControlId))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{assetPromise.Key} ControlId: {assetPromise.ControlId}).");
            }

            _controlIdToHandles.Remove(assetPromise.ControlId);
            Addressables.ReleaseInstance(assetPromise.GetResult<GameObject>());
        }

        private IAssetPromise LoadInternal<T>(string key, bool isAsync) where T : Object
        {
            var addressableHandle = Addressables.LoadAssetAsync<T>(key);
            if (!isAsync) addressableHandle.WaitForCompletion();

            var controlId = _nextControlId++;
            var setter = new AssetPromise(addressableHandle, key, controlId, false);
            _controlIdToHandles.Add(controlId, setter);

            addressableHandle.ToPromise(setter);

            return setter;
        }
      
        private IAssetPromise InstantiateInternal(string key, bool isAsync)
        {
            AsyncOperationHandle<GameObject> addressableHandle = Addressables.InstantiateAsync(key);
            if (!isAsync) addressableHandle.WaitForCompletion();

            var controlId = _nextControlId++;
            var setter = new AssetPromise(addressableHandle, key, controlId, true);
            _controlIdToHandles.Add(controlId, setter);

            addressableHandle.ToPromise(setter);
            return setter;
        }
       
        private void UnloadAssetPromise(IAssetPromise value)
        {
            if (value.IsInstantiate)
            {
                ReleaseInstance(value);
            }
            else
            {
                Unload<Object>(value);
            }
        }
    }
}
