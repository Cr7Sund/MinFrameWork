using System.Collections.Generic;
using System.Linq;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
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

        public IPromise<T> GetConfigAsync<T>(IAssetKey assetKey) where T : Object
        {
            if (_containers.ContainsKey(assetKey.Key))
            {
                return Promise<T>.Resolved(_containers[assetKey.Key].GetResult<T>());
            }
            else
            {
                return _assetLoader.LoadAsync<T>(assetKey) as IPromise<T>;
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
                return _assetLoader.LoadSync<T>(assetKey) as T;
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