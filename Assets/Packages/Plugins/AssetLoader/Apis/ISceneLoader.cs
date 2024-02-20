using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public interface ISceneLoader
    {
        IPromise LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        IPromise ActiveSceneAsync(IAssetKey key);
        void UnloadScene(IAssetKey key);

    }
}
