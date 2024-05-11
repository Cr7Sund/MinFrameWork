using System;
using System.Threading;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public interface ISceneLoader : IInitialize, IDisposable, ILateUpdate
    {
        PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode, bool activateOnLoad, CancellationToken cancellation);
        PromiseTask ActiveSceneAsync(IAssetKey key);
        PromiseTask UnloadScene(IAssetKey key);
        PromiseTask RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation);
    }
}
