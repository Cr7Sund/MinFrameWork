using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetContainer : IDisposable
    {
        IAssetPromise GetAsset<T>(IAssetKey key) where T : Object;
        T GetAssetSync<T>(IAssetKey key) where T : Object;
        IAssetPromise GetAssetAsync(IAssetKey assetKey);
        void Unload(IAssetKey key);
        bool ContainsAsset(IAssetKey key);
    }
}
