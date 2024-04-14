using System;
using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;


namespace Cr7Sund.Server.Api
{
    public interface IConfigContainer : IDisposable
    {
        PromiseTask<T> GetConfig<T>(IAssetKey assetKey) where T : Object;
        void RemoveConfigAsync(IAssetKey assetKey);
    }
}
