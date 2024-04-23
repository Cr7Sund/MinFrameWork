using System;
using System.Threading;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetLoader : IDisposable, IDestroyAsync
    {
        PromiseTask Init();
        PromiseTask<T> Load<T>(IAssetKey assetKey) ;
        PromiseTask<T> LoadAsync<T>(IAssetKey assetKey) ;

        void Unload(IAssetKey assetKey);
        void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation);
    }

}