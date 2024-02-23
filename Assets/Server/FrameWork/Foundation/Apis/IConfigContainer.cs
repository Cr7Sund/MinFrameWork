

using Cr7Sund.Package.Api;
using UnityEngine;

namespace Cr7Sund.Server.Api
{
    public interface IConfigContainer : IInitialize
    {
        IPromise<T> GetConfigAsync<T>(IAssetKey assetKey) where T : Object;
        T GetConfig<T>(IAssetKey assetKey) where T : Object;
    }
}
