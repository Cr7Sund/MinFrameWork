using Cr7Sund.Package.Api;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cr7Sund.AssetLoader.Api
{
    public interface IAssetPromise : IPromise<Object>
    {
        AsyncOperationHandle Handler { get; }
        string Key { get; }
        int ControlId { get; }
        bool IsInstantiate { get; }

        T GetResult<T>() where T : Object;
        T GetResultSync<T>() where T : Object;
        IPromise<T> GetConvertPromise<T>()where T : Object;
    }

}