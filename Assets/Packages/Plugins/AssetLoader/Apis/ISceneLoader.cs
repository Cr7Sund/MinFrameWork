using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public interface ISceneLoader
    {
        PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        PromiseTask ActiveSceneAsync(IAssetKey key);
        void UnloadScene(IAssetKey key);

    }
}
