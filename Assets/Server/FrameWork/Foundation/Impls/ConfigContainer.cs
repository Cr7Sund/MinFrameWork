using System.Collections.Generic;
using System.Linq;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Utils;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public class ConfigContainer : IConfigContainer
    {
        [Inject] private IAssetLoader _assetLoader;
        private Dictionary<string, IAssetPromise> _containers = new();

        public bool IsInit { get; private set; }


        public void Init()
        {
            if (!IsInit)
            {
                IsInit = true;
            }
        }

        public IAssetPromise GetConfigAsync(IAssetKey assetKey) 
        {
            if (_containers.ContainsKey(assetKey.Key))
            {
                return _containers[assetKey.Key];
            }
            else
            {
                var promise = _assetLoader.LoadAsync<Object>(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise;
            }
        }

        public T GetConfig<T>(IAssetKey assetKey) where T : Object
        {
            if (_containers.ContainsKey(assetKey.Key))
            {
                return _containers[assetKey.Key].GetResult<T>();
            }
            else
            {
                var promise = _assetLoader.LoadAsync<T>(assetKey);
                _containers.Add(assetKey.Key, promise);
                return promise.GetResult<T>();
            }
        }

        public void Dispose()
        {
            var promiseList = _containers.Values.ToList();

            for (int i = promiseList.Count - 1; i >= 0; i--)
            {
                _assetLoader.Unload<Object>(promiseList[i]);
            }
        }
    }
}