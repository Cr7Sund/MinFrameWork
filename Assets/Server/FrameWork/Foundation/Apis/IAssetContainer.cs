using System;
using System.Threading;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetContainer : IDisposable
    {
        PromiseTask<T> LoadAsset<T>(IAssetKey key) where T : Object;
        PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey, CancellationToken cancellation) where T : Object;
        PromiseTask Unload(IAssetKey key);
        PromiseTask UnloadAll();

    }
}
