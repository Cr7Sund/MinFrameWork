using Cr7Sund.Framework.Api;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetPromise : IPromise<Object>
    {
        AsyncOperationHandle Handler { get; }
        IAssetKey Key { get; }
        int ControlId { get; }
    }


}