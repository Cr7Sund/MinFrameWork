using System;
using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetContainer : IDisposable
    {
        IAssetPromise LoadAsset(IAssetKey key);
        IAssetPromise LoadAssetAsync(IAssetKey assetKey);
        [Obsolete]
        T GetAssetSync<T>(IAssetKey key) where T : Object;

        void Unload(IAssetKey key);
        bool ContainsAsset(IAssetKey key);
    }
}
