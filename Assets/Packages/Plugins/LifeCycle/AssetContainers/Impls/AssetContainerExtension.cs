using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.AssetContainers;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetContainers
{
    public static class AssetContainerExtension
    {
        public static async PromiseTask<Object[]> LoadGroup(this IAssetContainer container, IEnumerable<IAssetKey> assetKeys, UnsafeCancellationToken cancellation, IPoolBinder poolBinder)
        {
            var filterTasks = poolBinder.AutoCreate<List<PromiseTask<Object>>>();
            var filterKeys = poolBinder.AutoCreate<List<IAssetKey>>();

            var result = await ((BaseAssetContainer)container).ParallelLoadAssets(assetKeys, filterKeys, filterTasks, cancellation);
            poolBinder.Return<List<PromiseTask<Object>>, PromiseTask<Object>>(filterTasks);
            poolBinder.Return<List<IAssetKey>, IAssetKey>(filterKeys);
            return result;
        }

        public static async PromiseTask<T[]> LoadGroup<T>(this IAssetContainer container, IEnumerable<IAssetKey> assetKeys, UnsafeCancellationToken cancellation, IPoolBinder poolBinder) where T : Object
        {
            var filterTasks = poolBinder.AutoCreate<List<PromiseTask<T>>>();
            var filterKeys = poolBinder.AutoCreate<List<IAssetKey>>();

            var result = await ((BaseAssetContainer)container).ParallelLoadAssets(assetKeys, filterKeys, filterTasks, cancellation);
            poolBinder.Return<List<PromiseTask<T>>, PromiseTask<T>>(filterTasks);
            poolBinder.Return<List<IAssetKey>, IAssetKey>(filterKeys);
            return result;
        }
    }

}