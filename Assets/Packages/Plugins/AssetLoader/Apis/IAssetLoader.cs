using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IInitialize
    {
        IAssetPromise Load<T>(IAssetKey key) where T : Object;
        IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object;
        IAssetPromise InstantiateAsync(IAssetKey key) ;
        IAssetPromise Instantiate(IAssetKey key) ;
        void UnloadAsync<T>(IAssetPromise handler) where T : Object;
        void ReleaseInstance(IAssetPromise instance);
    }

}