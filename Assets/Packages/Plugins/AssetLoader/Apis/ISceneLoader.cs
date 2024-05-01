using System.Threading;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public interface ISceneLoader
    {
        PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode, bool activateOnLoad);
        PromiseTask ActiveSceneAsync(IAssetKey key);
        void UnloadScene(IAssetKey key);
        void RegisterCancelLoad(IAssetKey key, CancellationToken cancellation);

    }
}
