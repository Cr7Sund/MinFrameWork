using System;
using System.Threading;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IDisposable
    {
        PromiseTask Init();
        PromiseTask<T> Load<T>(IAssetKey key) where T : Object;
        PromiseTask<T> LoadAsync<T>(IAssetKey key) where T : Object;

        void Unload(IAssetKey handler);
        void RegisterCancelLoad(IAssetKey handler, CancellationToken cancellation);
    }

}