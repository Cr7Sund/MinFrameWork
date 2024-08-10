using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
using System.Diagnostics;
using Cr7Sund.CompilerServices;
namespace Cr7Sund.Package.Impl
{
    public class Promise<PromisedT> : IPromise<PromisedT>, IPromiseTaskSource<PromisedT>, IPoolNode<Promise<PromisedT>>
    {
        #region Static Fields
        private static readonly Promise<PromisedT> _resolvePromise = new Promise<PromisedT>();
        private static ReusablePool<ListPoolNode<ResolveHandler<PromisedT>>> _resolveListPool;
        private static ReusablePool<Promise<PromisedT>> _taskPool;
        #endregion

        #region Fields
        /// <summary>
        ///     Error handlers.
        /// </summary>
        protected ListPoolNode<RejectHandler> _rejectHandlers;
        /// <summary>
        ///     Completed handlers that accept no value.
        /// </summary>
        protected ListPoolNode<ResolveHandler<PromisedT>> _resolveHandlers;
        /// <summary>
        ///     Progress handlers.
        /// </summary>
        protected ListPoolNode<ProgressHandler> _progressHandlers;
        protected PromiseTaskCompletionSourceCore<PromisedT> _core;
        private Promise<PromisedT> _nextNode;
        private short _version;
        #endregion

        #region Properties
        public int Id
        {
            get;
            private set;
        }
        public string Name { get; protected set; }
        public PromiseState CurState { get; protected set; }
        public ref Promise<PromisedT> NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        private PromisedT _resolveValue => _core.Result;
        private Exception _rejectionException => _core.Error;
        public PromiseTask<PromisedT> Task
        {
            get
            {
                ValidateToken();
                return new PromiseTask<PromisedT>(this, _version);
            }
        }
        #endregion

        public Promise()
        {
            CurState = PromiseState.Pending;
            Id = Promise.NextId();


            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Add(this);
            }
        }

        public Promise(Action<Action<PromisedT>, Action<Exception>> resolver) : this()
        {
            try
            {
                resolver(Resolve, RejectWithoutDebug);
            }
            catch (Exception ex)
            {
                Reject(ex);
            }
        }

        protected Promise(PromiseState initialState) : this()
        {
            CurState = initialState;
        }

        public static Promise<PromisedT> Create()
        {
            if (!_taskPool.TryPop(out var promise))
            {
                promise = new Promise<PromisedT>();
            }
            else
            {
                promise.Id = Promise.NextId();
            }
            promise._version = promise._core.Version;
            return promise;
        }


        #region IPromiseInfo Implementation
        public IPromise<PromisedT> WithName(string name)
        {
            Name = name;
            return this;
        }

        public virtual void Dispose()
        {
            // Don't do that , since we only reference the value 
            // if (_resolveValue is IDisposable disposable
            //     && disposable != this)
            // {
            //     disposable?.Dispose();
            // }
            AssertUtil.IsNull(_resolveHandlers);
            AssertUtil.IsNull(_rejectHandlers);
            AssertUtil.IsNull(_progressHandlers);
            Name = string.Empty;
            CurState = PromiseState.Pending;
            _core.Reset();
            Id = -1;
        }

        public void TryReturn()
        {
            // TaskTracker.RemoveTracking(this);
            Dispose();
            _taskPool.TryPush(this);
        }
        
        #endregion

        #region IPromise Implementation
        public void Done(Action<PromisedT> onResolved, Action<Exception> onRejected)
        {
            Then(onResolved, onRejected)
                .Catch(ex =>
                    Promise.PropagateUnhandledException(this, ex)
                );
        }

        public void Done(Action<PromisedT> onResolved)
        {
            Then(onResolved)
                .Catch(ex =>
                    Promise.PropagateUnhandledException(this, ex)
                );
        }

        public virtual void Done()
        {
            Catch(ex => Promise.PropagateUnhandledException(this, ex));
        }

        public IPromise Catch(Action<Exception> onRejected)
        {
            AssertUtil.NotNull(onRejected);

            if (CurState == PromiseState.Resolved)
            {
                return Promise.Resolved();
            }

            var resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            void ResolveHandler(PromisedT _) => resultPromise.Resolve();

            void RejectHandler(Exception ex)
            {
                try
                {
                    onRejected(ex);
                    resultPromise.Resolve();
                }
                catch (Exception cbEx)
                {
                    resultPromise.Reject(cbEx);
                }
            }

            void ProgressHandler(float v) => resultPromise.ReportProgress(v);

            ActionHandlers(resultPromise, ResolveHandler, RejectHandler);
            ProgressHandlers(this, ProgressHandler);

            return resultPromise;
        }

        public IPromise<PromisedT> Catch(Func<Exception, PromisedT> onRejected)
        {
            if (CurState == PromiseState.Resolved)
            {
                return this;
            }

            var resultPromise = GetRawPromise<PromisedT>();
            resultPromise.WithName(Name);


            void RejectHandler(Exception e)
            {
                try
                {
                    resultPromise.Resolve(onRejected(e));
                }
                catch (Exception ex)
                {
                    resultPromise.Reject(ex);
                }
            }

            ActionHandlers(resultPromise, resultPromise.Resolve, RejectHandler);
            ProgressHandlers(resultPromise, resultPromise.ReportProgress);

            return resultPromise;
        }

        public IPromise Then(Action<PromisedT> onResolved)
        {
            return Then(onResolved, null, null);
        }

        public IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }

        public IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    onResolved(_resolveValue);
                    return Promise.Resolved();
                }
                catch (Exception ex)
                {
                    return Promise.Rejected(ex);
                }
            }

            Promise resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            Action<PromisedT> resolveHandler;
            if (onResolved != null)
            {
                resolveHandler = v =>
                {
                    onResolved(v);
                    resultPromise.Resolve();
                };
            }
            else
            {
                resolveHandler = _ => resultPromise.Resolve();
            }

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = ex =>
                {
                    onRejected(ex);
                    // we will catch the exception but still go on next operation of promise chain
                    resultPromise.Resolve();
                };
            }
            else
            {
                rejectHandler = resultPromise.RejectWithoutDebug;
            }

            ActionHandlers(resultPromise, resolveHandler, rejectHandler);

            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        public IPromise Then(Func<PromisedT, IPromise> onResolved)
        {
            return Then(onResolved, null, null);
        }

        public IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }

        public IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved(_resolveValue);
                }
                catch (Exception ex)
                {
                    return Promise.Rejected(ex);
                }
            }

            var resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            Action<PromisedT> resolveHandler;
            if (onResolved != null)
            {
                resolveHandler = v =>
                {
                    onResolved(v)
                        .Progress(progress => resultPromise.ReportProgress(progress))
                        .Then(resultPromise.Resolve, resultPromise.RejectWithoutDebug);
                };
            }
            else
            {
                resolveHandler = _ => resultPromise.Resolve();
            }

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = ex =>
                {
                    onRejected(ex);
                    // we will catch the exception but still go on next operation of promise chain
                    resultPromise.Resolve();
                };
            }
            else
            {
                rejectHandler = resultPromise.RejectWithoutDebug;
            }

            ActionHandlers(resultPromise, resolveHandler, rejectHandler);

            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved)
        {
            return Then(onResolved, null, null);
        }

        public IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved,
            Func<Exception, IPromise<ConvertedT>> onRejected
        )
        {
            return Then(onResolved, onRejected, null);
        }

        public IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved,
            Func<Exception, IPromise<ConvertedT>> onRejected,
            Action<float> onProgress
        )
        {
            AssertUtil.NotNull(onResolved, PromiseExceptionType.NO_UNRESOLVED);

            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved(_resolveValue);
                }
                catch (Exception ex)
                {
                    if (onRejected != null)
                    {
                        return onRejected.Invoke(ex);
                    }
                    else
                    {
                        return Promise<ConvertedT>.Rejected(ex);
                    }
                }
            }

            var resultPromise = GetRawPromise<ConvertedT>();
            resultPromise.WithName(Name);

            void ResolveHandler(PromisedT v)
            {
                onResolved(v)
                    .Progress(resultPromise.ReportProgress)
                    .Then(
                        // Should not be necessary to specify the arg type on the next line, but Unity (mono) has an public compiler error otherwise.
                        resultPromise.Resolve, resultPromise.RejectWithoutDebug);
            }

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = ex =>
                {
                    try
                    {
                        onRejected(ex)
                            .Then(
                                resultPromise.Resolve,
                                resultPromise.RejectWithoutDebug
                            );
                    }
                    catch (Exception callbackEx)
                    {
                        resultPromise.Reject(callbackEx);
                    }
                };
            }
            else
            {
                rejectHandler = resultPromise.RejectWithoutDebug;
            }

            ActionHandlers(resultPromise, ResolveHandler, rejectHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        [DebuggerHidden]
        public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform)
        {
            AssertUtil.NotNull(transform);
            return Then(value => Resolved(transform(value)));
        }

        public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(value => All(chain(value)));
        }

        public IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain)
        {
            return Then(value => Promise.All(chain(value)));
        }

        public IPromise<ConvertedT> ThenAny<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(value => Any(chain(value)));
        }

        public IPromise ThenAny(Func<PromisedT, IEnumerable<IPromise>> chain)
        {
            return Then(value => Promise.Any(chain(value)));
        }

        public IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(value => Race(chain(value)));
        }

        public IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain)
        {
            return Then(value => Promise.Race(chain(value)));
        }

        public IPromise<ConvertedT> ThenFirst<ConvertedT>(IEnumerable<Func<IPromise<ConvertedT>>> fns)
        {
            return Then(_ => First(fns));
        }
        public IPromise<PromisedT> Finally(Action onComplete)
        {
            AssertUtil.NotNull(onComplete);

            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    onComplete();
                    return this;
                }
                catch (Exception ex)
                {
                    return Promise<PromisedT>.Rejected(ex);
                }
            }

            var promise = GetRawPromise<PromisedT>();
            promise.WithName(Name);

            Then(promise.Resolve);
            Catch(e =>
            {
                // Something different from continue with
                // since we need to handle exception at last
                try
                {
                    onComplete();
                    promise.RejectWithoutDebug(e);
                }
                catch (Exception ne)
                {
                    promise.Reject(ne);
                }
            });

            return promise.Then(v =>
            {
                onComplete();
                return v;
            });
        }

        public IPromise ContinueWith(Func<IPromise> onComplete)
        {
            var promise = GetRawPromise();
            promise.WithName(Name);

            Then(_ => promise.Resolve());
            Catch(_ => promise.Resolve());

            return promise.Then(onComplete);
        }

        public IPromise ContinueWith(Action onComplete)
        {
            var promise = GetRawPromise();
            promise.WithName(Name);

            Then(_ => promise.Resolve());
            Catch(_ => promise.Resolve());

            return promise.Then(onComplete);
        }

        public IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete)
        {
            var promise = GetRawPromise();
            promise.WithName(Name);

            Then(_ => promise.Resolve());
            Catch(_ => promise.Resolve());
            return promise.Then(onComplete);
        }
        public IPromise<PromisedT> Progress(Action<float> onProgress)
        {
            if (CurState == PromiseState.Pending && onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }
            return this;
        }
        #endregion

        #region IPendingPromise
        public void Resolve(PromisedT value)
        {
            if (CurState != PromiseState.Pending)
            {
                throw new MyException(PromiseExceptionType.Valid_RESOLVED_STATE);
            }
            ValidateToken();

            CurState = PromiseState.Resolved;
            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeResolveHandlers(value);
            _core.TrySetResult(value);
        }

        public async PromiseTask<PromisedT> ResolveAsync(PromisedT value)
        {
            Resolve(value);
            return await Task;
        }

        public void ReportProgress(float progress)
        {
            if (CurState != PromiseState.Pending)
            {
                throw new MyException(PromiseExceptionType.Valid_PROGRESS_STATE);
            }

            InvokeProgressHandlers(progress);
        }

        public void Reject(Exception ex)
        {
            RejectWithoutDebug(ex);
            Console.Error(ex);
        }

        public void RejectWithoutDebug(Exception ex)
        {
            AssertUtil.NotNull(ex);
            if (CurState != PromiseState.Pending)
            {
                throw new MyException(PromiseExceptionType.Valid_REJECTED_STATE);
            }
            ValidateToken();

            CurState = PromiseState.Rejected;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeRejectHandlers(ex);
            _core.TrySetException(ex);
        }

        public virtual void Cancel()
        {
            RejectedWithoutDebug(new OperationCanceledException());
        }
        #endregion

        #region ITaskSource
        public PromisedT GetResult(short token)
        {
            try
            {
                return _core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        public void OnCompleted(Action continuation, short token)
        {
            _core.OnCompleted(continuation, token);
        }

        public PromiseTask<PromisedT> Join()
        {
            ValidateToken();

            switch (CurState)
            {
                case PromiseState.Pending:
                    var newPromise = Promise<PromisedT>.Create();
                    AddResolveHandler(newPromise.Resolve, newPromise);
                    AddRejectHandler(newPromise.RejectWithoutDebug, newPromise);
                    return newPromise.Task;
                case PromiseState.Rejected:
                default:
                    var tmpEx = _rejectionException;
                    throw new Exception(tmpEx.Message, tmpEx);
                case PromiseState.Resolved:
                    var tmpValue = _resolveValue;
                    return new PromiseTask<PromisedT>(tmpValue, _version);
            }
        }

        private void ValidateToken()
        {
            AssertUtil.AreEqual(this._version, this._core.Version, PromiseExceptionType.CAN_VISIT_VALID_VERSION);
        }

        #endregion

        #region private methods
        // Helper Function to invoke or register resolve/reject handlers
        protected void ActionHandlers(IRejectable resultPromise, Action<PromisedT> resolveHandler, Action<Exception> rejectHandler)
        {
            switch (CurState)
            {
                case PromiseState.Resolved:
                    InvokeHandler(resolveHandler, resultPromise, _resolveValue);
                    break;
                case PromiseState.Rejected:
                    InvokeHandler(rejectHandler, resultPromise, _rejectionException);
                    break;
                default:
                    AddResolveHandler(resolveHandler, resultPromise);
                    AddRejectHandler(rejectHandler, resultPromise);
                    break;
            }
        }

        // Helper function to invoke or register progress handlers.
        protected void ProgressHandlers(IRejectable resultPromise, Action<float> progressHandler)
        {
            if (CurState == PromiseState.Pending)
            {
                AddProgressHandler(progressHandler, resultPromise);
            }
        }

        protected void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
        {
            ValidateToken();
            if (_rejectHandlers == null)
            {
                _rejectHandlers = ListPoolNode<RejectHandler>.Create(ref Promise._rejectListPool);
            }

            _rejectHandlers.Add(new RejectHandler
            {
                Callback = onRejected,
                Rejectable = rejectable
            });
        }

        private void AddResolveHandler(Action<PromisedT> onResolved, IRejectable rejectable)
        {
            if (IsRecycled)
            {
                throw new Exception("can not add resolve handler, since it has been recycled");
            }
            if (_resolveHandlers == null)
            {
                _resolveHandlers = ListPoolNode<ResolveHandler<PromisedT>>.Create(ref _resolveListPool);
            }
            _resolveHandlers.Add(new ResolveHandler<PromisedT>
            {
                Callback = onResolved,
                Rejectable = rejectable
            });
        }


        private void AddProgressHandler(Action<float> onProgress, IRejectable rejectable)
        {
            if (IsRecycled)
            {
                throw new Exception("can not add progress handler, since it has been recycled");
            }
            if (_progressHandlers == null)
            {
                _progressHandlers = ListPoolNode<ProgressHandler>.Create(ref Promise._progressListPool);
            }
            _progressHandlers.Add(new ProgressHandler
            {
                Callback = onProgress,
                Rejectable = rejectable
            });
        }

        // Invoke all reject handlers.
        private void InvokeRejectHandlers(Exception ex)
        {
            if (_rejectHandlers != null)
            {
                for (int i = 0, count = _rejectHandlers.Count; i < count; i++)
                {
                    var handler = _rejectHandlers[i];
                    InvokeHandler(handler.Callback, handler.Rejectable, ex);
                }
            }

            ClearHandlers();
        }

        //Invoke all progress handlers.
        private void InvokeProgressHandlers(float progress)
        {
            if (_progressHandlers != null)
            {
                for (int i = 0, count = _progressHandlers.Count; i < count; i++)
                {
                    var handler = _progressHandlers[i];
                    InvokeHandler(handler.Callback, handler.Rejectable, progress);
                }
            }
        }

        // Invoke all resolve handlers
        protected virtual void InvokeResolveHandlers(PromisedT value)
        {
            if (_resolveHandlers != null)
            {
                for (int i = 0, count = _resolveHandlers.Count; i < count; i++)
                {
                    var handler = _resolveHandlers[i];
                    InvokeHandler(handler.Callback, handler.Rejectable, value);
                }
            }
            ClearHandlers();
        }

        protected virtual void ClearHandlers()
        {
            _rejectHandlers?.Clear();
            _resolveHandlers?.Clear();
            _progressHandlers?.Clear();

            if (_resolveHandlers != null)
            {
                _resolveHandlers.TryReturn(ref _resolveListPool);
            }
            if (_rejectHandlers != null)
            {
                _rejectHandlers.TryReturn(ref Promise._rejectListPool);
            }
            if (_progressHandlers != null)
            {
                _progressHandlers.TryReturn(ref Promise._progressListPool);
            }

            _rejectHandlers = null;
            _resolveHandlers = null;
            _progressHandlers = null;
        }

        protected void InvokeHandler<T>(Action<T> callback, IRejectable rejectable, T value)
        {
            AssertUtil.NotNull(callback);
            AssertUtil.NotNull(rejectable);

            try
            {
                callback(value);
            }
            catch (Exception ex)
            {
                rejectable.Reject(ex);
            }
        }
        #endregion

        #region Extension Method
        #region public extension method
        // Convert an exception directly into a rejected promise.
        public static IPromise<PromisedT> Rejected(Exception ex)
        {
            Console.Error(ex);
            return _resolvePromise.Rejected<PromisedT>(ex);
        }

        public static IPromise<PromisedT> Rejected(Enum errorCode)
        {
            Exception ex = new MyException(errorCode);
            Console.Error(ex);
            return _resolvePromise.Rejected<PromisedT>(ex);
        }

        public static IPromise<PromisedT> RejectedWithoutDebug(Exception ex)
        {
            return _resolvePromise.Rejected<PromisedT>(ex);
        }

        /// <summary>
        ///     Convert a simple value directly into a resolved promise.
        /// </summary>
        public static IPromise<PromisedT> Resolved(PromisedT promisedValue)
        {
            return _resolvePromise.Resolved(promisedValue);
        }

        public static IPromise<IEnumerable<PromisedT>> All(params IPromise<PromisedT>[] promises)
        {
            return _resolvePromise.All(promises);
        }
        public static IPromise<IEnumerable<PromisedT>> All(IEnumerable<IPromise<PromisedT>> promises)
        {
            return _resolvePromise.All(promises);
        }

        public static IPromise<PromisedT> Race(params IPromise<PromisedT>[] promises)
        {
            return _resolvePromise.Race(promises);
        }

        public static IPromise<PromisedT> Race(IEnumerable<IPromise<PromisedT>> promises)
        {
            return _resolvePromise.Race(promises);
        }

        public static IPromise<PromisedT> Any(params IPromise<PromisedT>[] promises)
        {
            return _resolvePromise.Any(promises);
        }

        public static IPromise<PromisedT> Any(IEnumerable<IPromise<PromisedT>> promises)
        {
            return _resolvePromise.Any(promises);
        }

        public static IPromise<PromisedT> First(params Func<IPromise<PromisedT>>[] fns)
        {
            return _resolvePromise.First(fns);
        }

        public static IPromise<PromisedT> First(IEnumerable<Func<IPromise<PromisedT>>> fns)
        {
            return _resolvePromise.First(fns);
        }
        #endregion

        #region protected extension method
        // all below methods can be consider as static methods
        // but we we want also support the inheritance to override

        protected virtual Promise<ConvertedT> GetRawPromise<ConvertedT>()
        {
            return new Promise<ConvertedT>();
        }

        protected virtual Promise GetRawPromise()
        {
            return new Promise();
        }

        // Convert an exception directly into a rejected promise.
        private IPromise<ConvertedT> Rejected<ConvertedT>(Exception ex)
        {
            AssertUtil.NotNull(ex);

            var promise = GetRawPromise<ConvertedT>();
            promise._core.TrySetException(ex);
            promise.CurState = PromiseState.Rejected;

            return promise;
        }

        /// <summary>
        ///     Convert a simple value directly into a resolved promise.
        /// </summary>
        private IPromise<ConvertedT> Resolved<ConvertedT>(ConvertedT promisedValue)
        {
            var promise = GetRawPromise<ConvertedT>();
            promise.CurState = PromiseState.Resolved;
            promise._core.TrySetResult(promisedValue);
            return promise;
        }

        /// <summary>
        ///     Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        ///     Returns a promise of a collection of the resolved results.
        /// </summary>
        private IPromise<IEnumerable<ConvertedT>> All<ConvertedT>(params IPromise<ConvertedT>[] promises)
        {
            return All((IEnumerable<IPromise<ConvertedT>>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        ///     Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        ///     Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise<IEnumerable<ConvertedT>> All<ConvertedT>(IEnumerable<IPromise<ConvertedT>> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                return Resolved(Enumerable.Empty<ConvertedT>());
            }

            int remainingCount = promisesArray.Length;
            var results = new ConvertedT[remainingCount];
            float[] progress = new float[remainingCount];
            var resultPromise = GetRawPromise<IEnumerable<ConvertedT>>();
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
            {
                promise
                    .Progress(v =>
                    {
                        progress[index] = v;
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.ReportProgress(progress.Average());
                        }
                    })
                    .Then(result =>
                        {
                            progress[index] = 1f;
                            results[index] = result;

                            --remainingCount;
                            if (remainingCount <= 0 && resultPromise.CurState == PromiseState.Pending)
                            {
                                // This will never happen if any of the promises errored.
                                resultPromise.Resolve(results);
                            }
                        },
                        ex =>
                        {
                            if (resultPromise.CurState == PromiseState.Pending)
                            {
                                // If a promise errored and the result promise is still pending, reject it.
                                resultPromise.RejectWithoutDebug(ex);
                            }
                        });
            });

            return resultPromise;
        }


        /// <summary>
        ///     Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        ///     otherwise, it will be rejected  when all of them are rejected
        ///     Returns a promise of a collection of the resolved results.
        /// </summary>
        private IPromise<ConvertedT> Any<ConvertedT>(params IPromise<ConvertedT>[] promises)
        {
            return Any((IEnumerable<IPromise<ConvertedT>>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        ///     Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        ///     otherwise, it will be rejected  when all of them are rejected
        ///     Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise<ConvertedT> Any<ConvertedT>(IEnumerable<IPromise<ConvertedT>> promises)
        {
            var promisesArray = promises.ToArray();
            AssertUtil.Greater(promisesArray.Length, 0,
                PromiseExceptionType.EMPTY_PROMISE_ANY);

            int remainingCount = promisesArray.Length;
            float[] progress = new float[remainingCount];
            var groupException = new PromiseGroupException(remainingCount);
            var resultPromise = GetRawPromise<ConvertedT>();
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
            {
                promise
                    .Progress(v =>
                    {
                        progress[index] = v;
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.ReportProgress(progress.Average());
                        }
                    })
                    .Then(
                        result =>
                        {
                            progress[index] = 1f;
                            resultPromise.ReportProgress(1f);

                            if (resultPromise.CurState == PromiseState.Pending)
                            {
                                // return first fulfill promise
                                resultPromise.Resolve(result);
                            }
                        },
                        ex =>
                        {
                            --remainingCount;
                            groupException.Exceptions[index] = ex;

                            if (remainingCount <= 0 && resultPromise.CurState == PromiseState.Pending)
                            {
                                // This will happen if all of the promises are rejected.
                                resultPromise.RejectWithoutDebug(groupException);
                            }
                        });
            });

            return resultPromise;
        }

        /// <summary>
        ///     Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        ///     Returns the value from the first promise that has resolved.
        /// </summary>
        private IPromise<ConvertedT> Race<ConvertedT>(params IPromise<ConvertedT>[] promises)
        {
            return Race((IEnumerable<IPromise<ConvertedT>>)promises); // Cast is required to force use of the other function.
        }

        /// <summary>
        ///     Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        ///     Returns the value from the first promise that has resolved.
        /// </summary>
        protected IPromise<ConvertedT> Race<ConvertedT>(IEnumerable<IPromise<ConvertedT>> promises)
        {
            var promisesArray = promises.ToArray();
            AssertUtil.Greater(promisesArray.Length, 0, PromiseExceptionType.EMPTY_PROMISE_RACE);

            int remainingCount = promisesArray.Length;
            var resultPromise = GetRawPromise<ConvertedT>();
            float[] progress = new float[remainingCount];
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
                {
                    promise
                        .Progress(v =>
                        {
                            progress[index] = v;
                            resultPromise.ReportProgress(progress.Max());
                        })
                        .Then(result =>
                            {
                                if (resultPromise.CurState == PromiseState.Pending)
                                {
                                    resultPromise.Resolve(result);
                                }
                            },
                            ex =>
                            {
                                if (resultPromise.CurState == PromiseState.Pending)
                                {
                                    // If a promise errored and the result promise is still pending, reject it.
                                    resultPromise.RejectWithoutDebug(ex);
                                }
                            });
                }
            );

            return resultPromise;
        }

        /// <summary>
        ///     Chain a number of operations using promises.
        ///     Returns the value of the first promise that resolves, or otherwise the exception thrown by the last operation.
        /// </summary>
        protected IPromise<ConvertedT> First<ConvertedT>(params Func<IPromise<ConvertedT>>[] fns)
        {
            return First((IEnumerable<Func<IPromise<ConvertedT>>>)fns);
        }

        /// <summary>
        ///     Chain a number of operations using promises.
        ///     Returns the value of the first promise that resolves, or otherwise the exception thrown by the last operation.
        /// </summary>
        private IPromise<ConvertedT> First<ConvertedT>(IEnumerable<Func<IPromise<ConvertedT>>> fns)
        {
            var promise = GetRawPromise<ConvertedT>();

            int count = 0;

            fns.Aggregate(
                    Promise<ConvertedT>.RejectedWithoutDebug(new Exception()),
                    (prevPromise, fn) =>
                    {
                        int itemSequence = count;
                        ++count;

                        var newPromise = GetRawPromise<ConvertedT>();
                        prevPromise
                            .Progress(v =>
                            {
                                float sliceLength = 1f / count;
                                promise.ReportProgress(sliceLength * (v + itemSequence));
                            })
                            .Then(newPromise.Resolve,
                                _ =>
                                {
                                    float sliceLength = 1f / count;
                                    promise.ReportProgress(sliceLength * itemSequence);

                                    fn()
                                        .Then(newPromise.Resolve)
                                        .Catch(newPromise.RejectWithoutDebug);
                                });
                        return newPromise;
                    })
                .Then(promise.Resolve)
                .Catch(ex =>
                {
                    promise.ReportProgress(1f);
                    promise.RejectWithoutDebug(ex);
                });

            return promise;
        }
        #endregion
        #endregion

        #region UnityTest
        public PromisedT Test_GetResolveValue()
        {
            return _resolveValue;
        }
        public static int Test_GetPoolCount()
        {
            return _taskPool.Size;
        }
        public static int Test_GetResolveListPoolCount()
        {
            return _resolveListPool.Size;
        }
        public void Test_ClearHandlers()
        {
            ClearHandlers();
        }
        #endregion
    }

    /// <summary>
    ///     Represents a handler invoked when the promise is resolved.
    /// </summary>
    public struct ResolveHandler<T>
    {
        /// <summary>
        ///     Callback fn.
        /// </summary>
        public Action<T> Callback;

        /// <summary>
        ///     The promise that is rejected when there is an error while invoking the handler.
        /// </summary>
        public IRejectable Rejectable;
    }
}
