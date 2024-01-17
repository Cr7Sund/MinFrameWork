using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Impl;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AssetPromise : Promise<Object>, IAssetPromise
    {
        public AsyncOperationHandle Handler { get; private set; }
        public IAssetKey Key { get; private set; }
        public int ControlId { get; private set; }

        public AssetPromise(AsyncOperationHandle handler, IAssetKey key, int controlId)
        {
            Handler = handler;
            Key = key;
            ControlId = controlId;
        }
    }
}