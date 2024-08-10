using Cr7Sund.Package.Api;

namespace Cr7Sund
{
    public static class PromiseStateExtension
    {
        public static Cr7Sund.PromiseTaskStatus ToTaskStatus(this PromiseState promiseState)
        {
            switch (promiseState)
            {
                case PromiseState.Pending: return Cr7Sund.PromiseTaskStatus.Pending;
                case PromiseState.Resolved: return Cr7Sund.PromiseTaskStatus.Succeeded;
                case PromiseState.Rejected:
                default:
                    return Cr7Sund.PromiseTaskStatus.Faulted;
            }
        }
    }
}
