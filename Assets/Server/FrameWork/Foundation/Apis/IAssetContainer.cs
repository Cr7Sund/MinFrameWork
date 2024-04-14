using System;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetContainer : IDisposable
    {
        PromiseTask<T> LoadAsset<T>(IAssetKey key) where T : Object;
        PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey) where T : Object;

        void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation);
        void Unload(IAssetKey key);
        void UnloadAll();

    }
}
