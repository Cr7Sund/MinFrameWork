using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetContainers
{
    public interface IAssetContainer : IDisposable
    {
        PromiseTask<T> LoadAsset<T>(IAssetKey key) where T : Object;
        PromiseTask<T> LoadAssetAsync<T>(IAssetKey assetKey, UnsafeCancellationToken cancellation) where T : Object;
        PromiseTask<T[]> ParallelLoadAssets<T>(IEnumerable<IAssetKey> assetKeys, IList<IAssetKey> inFilterKeys, IList<PromiseTask<T>> inFilterTasks, UnsafeCancellationToken cancellation) where T : Object;
        PromiseTask Unload(IAssetKey key);
        PromiseTask UnloadAll();
    }
}
