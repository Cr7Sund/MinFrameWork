using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cr7Sund.Server.Impl
{
    public static class AsyncOperationHandleExtension
    {

        public static PromiseTaskStatus ToTaskStatus(this AsyncOperationHandle handler)
        {
            if (handler.IsValid() && handler.IsDone)
            {
                return handler.Status == AsyncOperationStatus.Succeeded
                ? PromiseTaskStatus.Succeeded : PromiseTaskStatus.Faulted;
            }

            return PromiseTaskStatus.Pending;
        }


    }
}