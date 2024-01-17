using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IInitialize
    {
        IAssetPromise Load<T>(IAssetKey key) where T : Object;
        IAssetPromise LoadAsync<T>(IAssetKey key) where T : Object;
        void UnloadAsync<T>(IAssetPromise handler) where T : Object;
    }

}