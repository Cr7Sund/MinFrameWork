using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Impl;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AssetPromise : Promise<Object>, IAssetPromise
    {
        public AsyncOperationHandle Handler { get; private set; }
        public string Key { get; private set; }
        public int ControlId { get; private set; }
        public bool IsInstantiate { get; }

        public AssetPromise(AsyncOperationHandle handler, string key, int controlId, bool isInstantiate)
        {
            Handler = handler;
            Key = key;
            ControlId = controlId;
            IsInstantiate = isInstantiate;
        }

        public T GetResult<T>() where T : Object
        {
            if (Handler.Status == AsyncOperationStatus.Succeeded)
            {
                return Handler.Result as T;
            }
            else
            {
                return default(T);
            }
        }

        // public IPromise<T> ConvertPromise<T>() where T : Object
        // {
        //     if (Handler.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         return Promise<T>.Resolved(Handler.Result as T);
        //     }
        //     else
        //     {
        //         return Then((result) =>
        //         {
        //             return result as T;
        //         });
        //     }
        // }
    }
}