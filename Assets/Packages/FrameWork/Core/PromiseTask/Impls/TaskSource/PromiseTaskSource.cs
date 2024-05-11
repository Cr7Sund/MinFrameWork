using System;

namespace Cr7Sund
{
    public sealed class PromiseTaskSource<T> : IPromiseTaskSource<T>, ITaskPoolNode<PromiseTaskSource<T>>
    {
        private static TaskPool<PromiseTaskSource<T>> _pool;
       
        public PromiseTaskStatus _status;
        public T _result;
        public Action _registerAction;
        public Exception _error;
        public PromiseTaskSource<T> _nextNode;

        public ref PromiseTaskSource<T> NextNode => ref _nextNode;


        internal PromiseTaskSource(T value)
        {
            _result = value;
        }

        public static PromiseTaskSource<T> Create(T value)
        {
            if (!_pool.TryPop(out PromiseTaskSource<T> result))
            {
                result = new PromiseTaskSource<T>(value);
            }

            result._result = value;

            return result;
        }

        public T GetResult(short token)
        {
            var returnResult = _result;
            if (_error != null)
            {
                var tmpEx = _error;
                TryReturn();
                throw tmpEx;
            }

            TryReturn();
            return returnResult;
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
        }

        public void OnCompleted(Action continuation, short token)
        {
            _registerAction = continuation;
        }

        private void TryReturn()
        {
            _pool.TryPush(this);
            _result = default;
            _status = PromiseTaskStatus.Pending;
            _registerAction = null;
        }
    }
}
