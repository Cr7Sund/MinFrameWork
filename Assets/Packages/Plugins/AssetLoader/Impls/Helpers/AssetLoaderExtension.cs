

using UnityEngine;
using Cr7Sund.AssetLoader.Api;

namespace Cr7Sund.AssetLoader
{
    public static class AssetLoaderExtension
    {
        public static T LoadSync<T>(this IAssetLoader assetLoader, IAssetKey assetKey) where T : Object
        {
            var loadPromise = assetLoader.Load<T>(assetKey);
            return loadPromise.GetResult<T>();
        }

        public static T InstantiateSync<T>(this IAssetLoader assetLoader, IAssetKey assetKey) where T : Object
        {
            var loadPromise = assetLoader.Instantiate(assetKey);
            return loadPromise.GetResult<T>();
        }
    }
}