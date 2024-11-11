using System;
using System.Threading;
using UnityEngine.SceneManagement;

namespace Cr7Sund.AssetLoader.Api
{
    public interface ISceneLoader : IInitialize, IDisposable, ILateUpdatable
    {
        PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode, bool activateOnLoad, UnsafeCancellationToken cancellation);
        PromiseTask ActiveSceneAsync(IAssetKey key);
        PromiseTask UnloadScene(IAssetKey key);
    }
}
