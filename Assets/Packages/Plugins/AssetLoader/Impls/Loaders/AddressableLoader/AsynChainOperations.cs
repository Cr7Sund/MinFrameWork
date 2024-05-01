using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Cr7Sund.Server.Impl
{
    internal class AsynChainOperations : IPoolNode<AsynChainOperations>, IDisposable
    {
        private static ReusablePool<AsynChainOperations> _pool;

        private AsyncOperationHandle _handler;
        private readonly Action<AsyncOperationHandle> _completeAction;
        private Action<AsynChainOperations> _deleteAction;
        private List<PromiseTaskSource> _promiseTaskSources = new();
        private AsynChainOperations _nextNode;

        public ref AsynChainOperations NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        public AsyncOperationHandle Handler => _handler;


        public AsynChainOperations()
        {
            _completeAction = OnCompleted;
        }


        public PromiseTask Chain()
        {
            if (_handler.IsDone)
            {
                return PromiseTask.CompletedTask;
            }

            var taskSource = PromiseTaskSource.Create();
            _promiseTaskSources.Add(taskSource);
            return new PromiseTask(taskSource, 0);
        }

        public void Cancel(CancellationToken cancellation)
        {
            for (int i = _promiseTaskSources.Count - 1; i >= 0; i--)
            {
                PromiseTaskSource item = _promiseTaskSources[i];
                item.Cancel(cancellation);
            }

        }

        public static AsynChainOperations Start(ref AsyncOperationHandle handler)
        {
            if (!_pool.TryPop(out var wrapper))
            {
                wrapper = new AsynChainOperations();
            }

            wrapper._handler = handler;
            if (!handler.IsDone)
            {
                handler.Completed += wrapper._completeAction;
            }

            return wrapper;
        }

        public T GetResult<T>()
        {
            if (_handler.IsValid() && _handler.IsDone)
            {
                AssertUtil.NotNull(_handler.Result);
                AssertUtil.IsInstanceOf(typeof(T), _handler.Result);
                return (T)_handler.Result;
            }
            else
            {
                throw new MyException(AssetLoaderExceptionType.no_done_State);
            }
        }

        public void Unload(Action<AsynChainOperations> deleteAction)
        {
            if (_promiseTaskSources.Count == 0)
            {
                deleteAction?.Invoke(this);
            }
            else
            {
                // delay delate 
                _deleteAction = deleteAction;
            }
        }

        public void Dispose()
        {
            AssertUtil.LessOrEqual(_promiseTaskSources.Count, 0);
            _deleteAction = null;
            _handler = default;
        }

        private void OnCompleted(AsyncOperationHandle h)
        {
            h.Completed -= OnCompleted;
            for (int i = _promiseTaskSources.Count - 1; i >= 0; i--)
            {
                PromiseTaskSource item = _promiseTaskSources[i];
                // item.status = h.ToTaskStatus();
                item.Resolve();
            }

            _promiseTaskSources.Clear();
            _deleteAction?.Invoke(this);
        }

        public void TryReturn()
        {
            Dispose();
            _pool.TryPush(this);
        }
    }
}
