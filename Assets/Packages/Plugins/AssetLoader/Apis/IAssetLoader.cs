using System;
using System.Threading;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IDisposable, IDestroyAsync
    {
        PromiseTask Init();
        PromiseTask<T> Load<T>(IAssetKey assetKey) where T : Object;
        PromiseTask<T> LoadAsync<T>(IAssetKey assetKey, UnsafeCancellationToken cancellation) where T : Object;
        PromiseTask Unload(IAssetKey assetKey);

    }

}