using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader
{
    public static class UnityAsyncOperationHandleExtension
    {
        public static void ToPromise<T>(this AsyncOperationHandle<T> addressableHandle, AssetPromise setter) where T : Object
        {
            AssertUtil.IsTrue(addressableHandle.IsValid());

            if (addressableHandle.Status != AsyncOperationStatus.None)
            {
                OnCompleted(addressableHandle, setter);
            }

            addressableHandle.Completed += x =>
            {
                OnCompleted(x, setter);
            };
        }

        private static void OnCompleted<T>(AsyncOperationHandle<T> addressableHandle, AssetPromise setter) where T : Object
        {
            if (addressableHandle.Status == AsyncOperationStatus.Succeeded)
            {
                setter.Resolve(addressableHandle.Result);
            }
            else if (addressableHandle.Status == AsyncOperationStatus.Failed)
            {
                setter.Reject(addressableHandle.OperationException);
            }
        }
    }
}