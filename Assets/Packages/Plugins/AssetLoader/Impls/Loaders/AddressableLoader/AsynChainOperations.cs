using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Cr7Sund.Server.Impl
{
    internal class AsyncChainOperations : IPoolNode<AsyncChainOperations>, IDisposable
    {
        private static ReusablePool<AsyncChainOperations> _pool;

        private AsyncOperationHandle _handler;
        private readonly Action<AsyncOperationHandle> _completeAction;
        private Action<AsyncChainOperations> _deleteAction;
        private List<PromiseTaskSource> _promiseTaskSources = new();
        private AsyncChainOperations _nextNode;

        public ref AsyncChainOperations NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        public AsyncOperationHandle Handler => _handler;

        public AsyncChainOperations()
        {
            _completeAction = OnCompleted;
        }


        public PromiseTask Chain()
        {
            if (!_handler.IsValid())
            {
                return PromiseTask.CompletedTask;
            }
            if (_handler.IsDone)
            {
                return PromiseTask.CompletedTask;
            }

            var taskSource = PromiseTaskSource.Create();
            _promiseTaskSources.Add(taskSource);
            return new PromiseTask(taskSource, 0);
        }

        public void RegisterCancel(CancellationToken cancellation)
        {
            for (int i = 0; i < _promiseTaskSources.Count; i++)
            {
                PromiseTaskSource item = _promiseTaskSources[i];
                item.RegisterCancel(cancellation);
            }
        }

        public static AsyncChainOperations Start(ref AsyncOperationHandle handler, CancellationToken cancellation)
        {
            if (!_pool.TryPop(out var wrapper))
            {
                wrapper = new AsyncChainOperations();
            }

            wrapper._handler = handler;
            if (!handler.IsDone)
            {
                handler.Completed += wrapper._completeAction;
            }

            if (cancellation.IsCancellationRequested)
            {
                wrapper.RegisterCancel(cancellation);
            }
            else
            {
                cancellation.Register(() =>
                {
                    wrapper.RegisterCancel(cancellation);
                });
            }

            return wrapper;
        }

        public T GetResult<T>()
        {
            if (!_handler.IsDone)
            {
                throw new MyException(AssetLoaderExceptionType.no_done_State);
            }

            if (_handler.IsValid())
            {
                AssertUtil.NotNull(_handler.Result);
                AssertUtil.IsInstanceOf(typeof(T), _handler.Result);

                return (T)_handler.Result;
            }
            else
            {
                return default;
            }
        }

        public void Unload(Action<AsyncChainOperations> deleteAction)
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
            for (int i = 0; i < _promiseTaskSources.Count; i++)
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
