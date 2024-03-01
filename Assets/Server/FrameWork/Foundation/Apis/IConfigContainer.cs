

using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using UnityEngine;

namespace Cr7Sund.Server.Api
{
    public interface IConfigContainer : IInitialize
    {
        IAssetPromise GetConfigAsync(IAssetKey assetKey);
        T GetConfig<T>(IAssetKey assetKey) where T : Object;
    }
}
