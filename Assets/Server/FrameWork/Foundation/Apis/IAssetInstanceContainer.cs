using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetInstanceContainer : IAssetContainer
    {
        IAssetPromise CreateInstance<T>(IAssetKey key) where T : Object;
        T CreateInstanceSync<T>(IAssetKey key) where T : Object;
        IAssetPromise CreateInstanceAsync<T>(IAssetKey key) where T : Object;

        bool TryGetInstance<T>(IAssetKey key, out T result) where T : Object;
    }
}
