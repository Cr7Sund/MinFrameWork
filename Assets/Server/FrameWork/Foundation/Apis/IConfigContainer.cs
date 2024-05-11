using System;
using Object = UnityEngine.Object;


namespace Cr7Sund.Server.Api
{
    public interface IConfigContainer : IAssetContainer, IDisposable
    {
        PromiseTask<T> GetConfig<T>(IAssetKey assetKey) where T : Object;
        void RemoveConfigAsync(IAssetKey assetKey);
    }
}
