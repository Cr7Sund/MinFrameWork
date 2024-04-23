using System;
using System.Threading;

namespace Cr7Sund
{
    public sealed class CanceledResultSource : IPromiseTaskSource
    {
        readonly CancellationToken cancellationToken;

        public CanceledResultSource(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public void GetResult(short token)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return PromiseTaskStatus.Canceled;
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return PromiseTaskStatus.Canceled;
        }

        public void OnCompleted(Action continuation, short token)
        {
            continuation();
        }
    }

    public sealed class CanceledResultSource<T> : IPromiseTaskSource<T>
    {
        readonly CancellationToken cancellationToken;

        public CanceledResultSource(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public T GetResult(short token)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return PromiseTaskStatus.Canceled;
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return PromiseTaskStatus.Canceled;
        }


        public void OnCompleted(Action continuation, short token)
        {
            continuation();
        }
    }

    public static class CanceledUniTaskCache<T>
    {
        public static readonly PromiseTask<T> Task;

        static CanceledUniTaskCache()
        {
            Task = new PromiseTask<T>(new CanceledResultSource<T>(CancellationToken.None), 0);
        }
    }

    public static class CanceledUniTaskCache
    {
        public static readonly PromiseTask Task;

        static CanceledUniTaskCache()
        {
            Task = new PromiseTask(new CanceledResultSource(CancellationToken.None), 0);
        }
    }
}