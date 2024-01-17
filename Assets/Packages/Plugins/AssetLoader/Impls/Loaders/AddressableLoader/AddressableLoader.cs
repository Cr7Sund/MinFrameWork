

using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Util;
using UnityEngine.AddressableAssets;
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

        public IAssetPromise Load<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key, false);
        }
        public IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object
        {
            return LoadInternal<T>(key, true);
        }
        public void UnloadAsync<T>(IAssetPromise handler) where T : Object
        {
            if (!_controlIdToHandles.ContainsKey(handler.ControlId))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{handler.Key} ControlId: {handler.ControlId}).");
            }

            var addressableHandle = _controlIdToHandles[handler.ControlId];
            _controlIdToHandles.Remove(handler.ControlId);
            Addressables.Release(addressableHandle);
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

    }
}