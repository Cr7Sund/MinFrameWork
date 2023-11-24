using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class Promise<PromisedT> : IPromise<PromisedT>
    {

        #region Fields
        /// <summary>
        ///     The exception when the promise is rejected.
        /// </summary>
        private Exception _rejectionException;

        /// <summary>
        ///     Error handlers.
        /// </summary>
        protected List<RejectHandler> _rejectHandlers;
        /// <summary>
        ///     Completed handlers that accept no value.
        /// </summary>
        protected List<ResolveHandler<PromisedT>> _resolveHandlers;
        /// <summary>
        ///     Progress handlers.
        /// </summary>
        protected List<ProgressHandler> _progressHandlers;

        /// <summary>
        ///     The value when the promises is resolved.
        /// </summary>
        protected PromisedT _resolveValue;


        private static readonly Promise<PromisedT> _resolvePromise = new Promise<PromisedT>();
        #endregion

        #region Properties
        public int Id
        {
            get;
        }

        public object Name { get; protected set; }
        public PromiseState CurState { get; protected set; }
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
                resolver(Resolve, Reject);
            }
            catch (Exception ex)
            {
                Reject(ex);
            }
        }

        protected Promise(PromiseState initialState)
        {
            CurState = initialState;
            Id = Promise.NextId();
        }

        public PromisedT Test_GetResolveValue()
        {
            return _resolveValue;
        }

        #region IPromiseInfo Implementation
        public IPromise<PromisedT> WithName(object name)
        {
            Name = name;
            return this;
        }

        public virtual void Dispose()
        {
            ClearHandlers();
            Name = string.Empty;
            _resolveValue = default;
            CurState = PromiseState.Pending;
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

        public virtual IDisposable Done()
        {
           return Catch(ex => Promise.PropagateUnhandledException(this, ex));
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

            void ResolveHandler(PromisedT v) => resultPromise.Resolve(v);

            void RejectHandler(Exception ex)
            {
                try
                {
                    resultPromise.Resolve(onRejected(ex));
                }
                catch (Exception cbEx)
                {
                    resultPromise.Reject(cbEx);
                }
            }

            void ProgressHandler(float v) => resultPromise.ReportProgress(v);

            ActionHandlers(resultPromise, ResolveHandler, RejectHandler);
            ProgressHandlers(resultPromise, ProgressHandler);

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

            var resultPromise = GetRawPromise();
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
                resolveHandler = resultPromise.ResolveWrap;
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
                rejectHandler = resultPromise.Reject;
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
                        .Then(
                            () => resultPromise.Resolve(),
                            ex => resultPromise.Reject(ex)
                        );
                };
            }
            else
            {
                resolveHandler = resultPromise.ResolveWrap;
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
                rejectHandler = resultPromise.Reject;
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

            AssertUtil.NotNull(onResolved, new PromiseException(
                "onResolved must be supplied", PromiseExceptionType.NO_UNRESOLVED));

            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved(_resolveValue);
                }
                catch (Exception ex)
                {
                    return Rejected<ConvertedT>(ex);
                }
            }

            var resultPromise = GetRawPromise<ConvertedT>();
            resultPromise.WithName(Name);

            void ResolveHandler(PromisedT v)
            {
                onResolved(v)
                    .Progress(progress => resultPromise.ReportProgress(progress))
                    .Then(
                        // Should not be necessary to specify the arg type on the next line, but Unity (mono) has an internal compiler error otherwise.
                        chainedValue => resultPromise.Resolve(chainedValue), ex => resultPromise.Reject(ex));
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
                                chainedValue => resultPromise.Resolve(chainedValue),
                                callbackEx => resultPromise.Reject(callbackEx)
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
                rejectHandler = resultPromise.Reject;
            }

            ActionHandlers(resultPromise, ResolveHandler, rejectHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

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
                    return Rejected<PromisedT>(ex);
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
                    promise.Reject(e);
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
            AssertUtil.AreEqual(PromiseState.Pending, CurState,
                new PromiseException(
                    "Attempt to resolve a promise that is already in state: " + CurState
                                                                              + ", a promise can only be resolved when it is still in state: "
                                                                              + PromiseState.Pending, PromiseExceptionType.Valid_STATE
                ));

            _resolveValue = value;
            CurState = PromiseState.Resolved;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeResolveHandlers(value);
        }

        public void ReportProgress(float progress)
        {
            AssertUtil.AreEqual(PromiseState.Pending, CurState,
                new PromiseException(
                    "Attempt to report progress a promise that is already in state: " + CurState
                                                                                      + ", a promise can only be resolved when it is still in state: "
                                                                                      + PromiseState.Pending, PromiseExceptionType.Valid_STATE
                ));


            InvokeProgressHandlers(progress);
        }

        public void Reject(Exception ex)
        {
            AssertUtil.NotNull(ex);
            AssertUtil.AreEqual(PromiseState.Pending, CurState,
                new PromiseException(
                    "Attempt to rejected a promise that is already in state: " + CurState
                                                                               + ", a promise can only be resolved when it is still in state: "
                                                                               + PromiseState.Pending, PromiseExceptionType.Valid_STATE
                ));


            _rejectionException = ex;
            CurState = PromiseState.Rejected;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeRejectHandlers(ex);
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
            _rejectHandlers ??= new List<RejectHandler>();

            _rejectHandlers.Add(new RejectHandler
            {
                Callback = onRejected,
                Rejectable = rejectable
            });
        }

        private void AddResolveHandler(Action<PromisedT> onResolved, IRejectable rejectable)
        {
            _resolveHandlers ??= new List<ResolveHandler<PromisedT>>();

            _resolveHandlers.Add(new ResolveHandler<PromisedT>
            {
                Callback = onResolved,
                Rejectable = rejectable
            });
        }

        private void AddProgressHandler(Action<float> onProgress, IRejectable rejectable)
        {
            _progressHandlers ??= new List<ProgressHandler>();

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
                for (int i = 0; i < _rejectHandlers.Count; i++)
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
                for (int i = 0; i < _progressHandlers.Count; i++)
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
                for (int i = 0; i < _resolveHandlers.Count; i++)
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
            promise.CurState = PromiseState.Rejected;
            promise._rejectionException = ex;
            return promise;
        }

        /// <summary>
        ///     Convert a simple value directly into a resolved promise.
        /// </summary>
        private IPromise<ConvertedT> Resolved<ConvertedT>(ConvertedT promisedValue)
        {
            var promise = GetRawPromise<ConvertedT>();
            promise.CurState = PromiseState.Resolved;
            promise._resolveValue = promisedValue;
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
                    })
                    .Catch(ex =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // If a promise errored and the result promise is still pending, reject it.
                            resultPromise.Reject(ex);
                        }
                    })
                    .Done();
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
            if (promisesArray.Length == 0)
            {
                throw new PromiseException(
                    "At least 1 input promise must be provided for any",
                    PromiseExceptionType.EMPTY_PROMISE_ANY
                );
            }

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
                    .Then(result =>
                    {
                        progress[index] = 1f;
                        resultPromise.ReportProgress(1f);

                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // return first fulfill promise
                            resultPromise.Resolve(result);
                        }
                    })
                    .Catch(ex =>
                    {
                        --remainingCount;
                        groupException.Exceptions[index] = ex;

                        if (remainingCount <= 0 && resultPromise.CurState == PromiseState.Pending)
                        {
                            // This will happen if all of the promises are rejected.
                            resultPromise.Reject(groupException);
                        }
                    })
                    .Done();
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
            if (promisesArray.Length == 0)
            {
                throw new PromiseException(
                    "At least 1 input promise must be provided for Race",
                    PromiseExceptionType.EMPTY_PROMISE_RACE
                );
            }
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
                        })
                        .Catch(ex =>
                        {
                            if (resultPromise.CurState == PromiseState.Pending)
                            {
                                // If a promise errored and the result promise is still pending, reject it.
                                resultPromise.Reject(ex);
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
                    Rejected<ConvertedT>(new Exception()),
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
                            .Then(newPromise.Resolve)
                            .Catch(_ =>
                            {
                                float sliceLength = 1f / count;
                                promise.ReportProgress(sliceLength * itemSequence);

                                fn()
                                    .Then(value => newPromise.Resolve(value))
                                    .Catch(newPromise.Reject);
                            })
                            ;
                        return newPromise;
                    })
                .Then(value => promise.Resolve(value))
                .Catch(ex =>
                {
                    promise.ReportProgress(1f);
                    promise.Reject(ex);
                });

            return promise;
        }
        #endregion
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
