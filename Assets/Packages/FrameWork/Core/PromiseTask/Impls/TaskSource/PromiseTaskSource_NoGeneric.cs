using System;
using System.Threading;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund
{
    public sealed class PromiseTaskSource : IPromiseTaskSource, IPoolNode<PromiseTaskSource>
    {
        private static ReusablePool<PromiseTaskSource> _pool;

        private PromiseTaskStatus _status;
        private Action _registerAction;
        private PromiseTaskSource _nextNode;
        public Exception _error;

        public ref PromiseTaskSource NextNode => ref _nextNode;
        // private PromiseTaskStatus Status => _status;
        public bool IsRecycled { get; set; }


        public static PromiseTaskSource Create()
        {
            if (!_pool.TryPop(out PromiseTaskSource result))
            {
                result = new PromiseTaskSource();
            }

            return result;
        }

        public void GetResult(short token)
        {
            if (_error != null)
            {
                var tmpEx = _error;
                TryReturn();
                throw tmpEx;
            }

            TryReturn();
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return _status;
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return _status;
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            if (_error != null)
            {
                var tmpEx = _error;
                TryReturn();
                throw tmpEx;
            }

            TryReturn();
        }

        public void OnCompleted(Action continuation, short token)
        {
            _registerAction = continuation;
        }

        public void Cancel(CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
            {
                Reject(new OperationCanceledException(cancellation));
            }
        }

        public void Resolve()
        {
            if (_error != null)
            {
                _status = PromiseTaskStatus.Succeeded;
            }

            _registerAction?.Invoke();
        }

        public void Reject(Exception e)
        {
            _error = e;
            if (e is OperationCanceledException)
            {
                _status = PromiseTaskStatus.Canceled;
            }
            else
            {
                _status = PromiseTaskStatus.Faulted;
            }
            _registerAction?.Invoke();
        }

        private void TryReturn()
        {
            _status = PromiseTaskStatus.Pending;
            _registerAction = null;
            _error = null;
            _pool.TryPush(this);
        }
    }
}
