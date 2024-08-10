using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AsyncChainOperations<T> : IPoolNode<AsyncChainOperations<T>>, IDisposable
    {
        private enum State
        {
            DefaultState,
            NotifyingState,
            NotifyingCompleteState,
        }

        private static ReusablePool<AsyncChainOperations<T>> _pool;

        private readonly List<PromiseTaskSource> _promiseTaskSources = new();
        private readonly Action<AsyncOperationHandle<T>> _completedAction;
        private AsyncOperationHandle<T> _handler;
        private Action<AsyncChainOperations<T>> _deleteAction;
        private AsyncChainOperations<T> _nextNode;
        private UnsafeCancellationToken _cancellation;
        private string _cancelMsg;
        private State _state;
        
        public ref AsyncChainOperations<T> NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        public AsyncOperationHandle Handler => _handler;


        public AsyncChainOperations()
        {
            _completedAction = OnCompleted;
        }

        public static AsyncChainOperations<T> Start(ref AsyncOperationHandle<T> handler, string cancelMsg, UnsafeCancellationToken cancellation)
        {
            if (!_pool.TryPop(out var wrapper))
            {
                wrapper = new AsyncChainOperations<T>();
            }

            wrapper._handler = handler;
            wrapper._cancellation = cancellation;
            wrapper._cancelMsg = cancelMsg;
            wrapper._state = State.DefaultState;

            if (!handler.IsDone)
            {
                handler.Completed += wrapper._completedAction;
            }

            return wrapper;
        }


        public PromiseTask Chain()
        {
            // we should wait until the handler is done
            // otherwise we should add operation into waiting list
            // if (_cancellation.IsCancellationRequested)
            // {
            //     return PromiseTask.FromCanceled(_cancelMsg, _cancellation);
            // }

            if (!_handler.IsValid())
            {
                return PromiseTask.CompletedTask;
            }
            if (_handler.IsDone)
            {
                return PromiseTask.CompletedTask;
            }

            Validate();
            var taskSource = PromiseTaskSource.Create();
            _promiseTaskSources.Add(taskSource);
            return taskSource.Task;
        }

        /// <summary>
        /// cancel immediately 
        /// but we still need to handle the unload when asset loaded callback return
        /// </summary>
        /// <param name="cancelMsg"></param>
        /// <param name="cancellation"></param>
        public void RegisterCancel(string cancelMsg, UnsafeCancellationToken cancellation)
        {
            if (_state == State.NotifyingState)
            {
                return;
            }

            int count = _promiseTaskSources.Count;
            for (int i = 0; i < count; i++)
            {
                PromiseTaskSource item = _promiseTaskSources[i];
                item.TryCancel(cancelMsg, cancellation);
            }
            _promiseTaskSources.Clear();
        }

        public TResult GetResult<TResult>() where TResult : T
        {
            if (!_handler.IsDone)
            {
                throw new MyException(AssetLoaderExceptionType.no_done_State);
            }

            // no sense: since await unload but cancel
            // if (_cancellation.IsValid && _cancellation.IsCancellationRequested)
            // {
            //     throw new MyException(AssetLoaderExceptionType.cancel_state);
            // }

            if (_handler.IsValid())
            {
                AssertUtil.NotNull(_handler.Result);
                AssertUtil.IsInstanceOf(typeof(TResult), _handler.Result);

                return (TResult)_handler.Result;
            }
            else
            {
                throw new MyException(AssetLoaderExceptionType.fail_state);
            }
        }

        public void RegisterUnload(Action<AsyncChainOperations<T>> deleteAction)
        {
            // make sure don't delete twice or more 
            // if state is done, delete action is been call first
            // if state is not done, we should wait until done
            AssertUtil.IsNull(_deleteAction, AssetLoaderExceptionType.unload_duplicate);

            // delay destroy after all actions be done
            // and handler is done
            if (_state == State.NotifyingCompleteState)              // wait until async operation callback 
            {
                deleteAction?.Invoke(this);
            }
            else
            {
                // delay delate 
                _deleteAction = deleteAction;
            }
        }

        public void TryReturn()
        {
            Dispose();
            _pool.TryPush(this);
        }

        public void Dispose()
        {
            AssertUtil.LessOrEqual(_promiseTaskSources.Count, 0, AssetLoaderExceptionType.Left_UnResolved_Handlers);
            AssertUtil.IsFalse(State.NotifyingState == _state);
            _deleteAction = null;
            _handler = default;
            _cancellation = UnsafeCancellationToken.None;
        }

        private void OnCompleted(AsyncOperationHandle<T> handler)
        {
            _state = State.NotifyingState;
            handler.Completed -= _completedAction;

            int count = _promiseTaskSources.Count;
            for (int i = 0; i < count; i++)
            {
                PromiseTaskSource item = _promiseTaskSources[i];
                if (!_cancellation.IsCancellationRequested)
                {
                    item.TryResolve();
                }
                else
                {
                    item.TryCancel(_cancelMsg, _cancellation);
                }
            }
            _promiseTaskSources.Clear();
            _state = State.NotifyingCompleteState;
            _deleteAction?.Invoke(this);
        }

        private bool Validate()
        {
            if (_state == State.NotifyingState)
            {
                throw new Exception("can modify chain when notifying");
            }

            return true;
        }
    }
}
