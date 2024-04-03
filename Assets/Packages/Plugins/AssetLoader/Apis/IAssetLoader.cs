using System;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IInitialize, IDisposable
    {
        IAssetPromise Load<T>(IAssetKey key) where T : Object;
        IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object;

        void Unload(IAssetPromise handler);
    }

}