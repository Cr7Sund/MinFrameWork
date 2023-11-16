using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using NUnit.Framework;
using System.Linq;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Impl
{
    public class Promise : IPromise
    {
        #region Fields

        #region  Static Fields
        /// <summary>
        /// Set to true to enable tracking of promises.
        /// </summary>
        public static bool EnablePromiseTracking = false;
        /// <summary>
        /// Event raised for unhandled errors.
        /// For this to work you have to complete your promises with a call to Done().
        /// </summary>
        public static event EventHandler<ExceptionEventArgs> UnhandledException
        {
            add { unhandlerException += value; }
            remove { unhandlerException -= value; }
        }
        private static EventHandler<ExceptionEventArgs> unhandlerException;

        /// <summary>
        /// Id for the next promise that is created.
        /// </summary>
        private static int nextPromiseId;

        /// <summary>
        /// Information about pending promises.
        /// </summary>
        internal static readonly HashSet<IPromiseInfo> PendingPromises =
            new HashSet<IPromiseInfo>();

        // for unit-test
        public static int GetPendingPromiseCount() => PendingPromises.Count;

        public static void ClearPending() => PendingPromises.Clear();
        /// <summary>
        /// Convert a simple value directly into a resolved promise.
        /// </summary>
        private static Promise resolvedPromise = new Promise(PromiseState.Resolved);

        #endregion

        /// <summary>
        /// The exception when the promise is rejected.
        /// </summary>
        private Exception rejectionException;

        /// <summary>
        /// Error handlers.
        /// </summary>
        private List<RejectHandler> rejectHandlers;
        /// <summary>
        /// Completed handlers that accept no value.
        /// </summary>
        private List<ResolveHandler> resolveHandlers;
        /// <summary>
        /// Progress handlers.
        /// </summary>
        private List<ProgressHandler> progressHandlers;

        private readonly int id;

        #endregion

        #region  Properties

        public int Id { get { return id; } }

        public object Name { get; protected set; }
        public PromiseState CurState { get; private set; }


        #endregion


        public Promise()
        {
            this.CurState = PromiseState.Pending;
            this.id = NextId();
            if (EnablePromiseTracking)
            {
                PendingPromises.Add(this);
            }
        }

        public Promise(Action<Action, Action<Exception>> resolver) : this()
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
            id = NextId();
        }

        #region IPromiseInfo Implementation

        public IPromise WithName(object name)
        {
            this.Name = name;
            return this;
        }

        internal static int NextId()
        {
            return ++nextPromiseId;
        }

        #endregion

        #region IPromise
        public void Done(Action onResolved, Action<Exception> onRejected)
        {
            Then(onResolved, onRejected)
                .Catch(ex =>
                     PropagateUnhandledException(this, ex)
                );
        }

        public void Done(Action onResolved)
        {
            Then(onResolved)
                .Catch(ex =>
                    PropagateUnhandledException(this, ex)
                );
        }

        public void Done()
        {
            Catch(ex => PropagateUnhandledException(this, ex));
        }

        public IPromise Catch(Action<Exception> onRejected)
        {
            Assert.NotNull(onRejected);

            if (CurState == PromiseState.Resolved)
            {
                return this;
            }

            var resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            Action resolveHandler = () => resultPromise.Resolve();
            Action<Exception> rejectHandler = ex =>
            {
                try
                {
                    onRejected(ex);
                    resultPromise.Resolve();
                }
                catch (Exception callbackException)
                {
                    resultPromise.Reject(callbackException);
                }
            };

            ActionHandlers(resultPromise, resolveHandler, rejectHandler);
            ProgressHandlers(this, v => resultPromise.ReportProgress(v));

            return resultPromise;
        }

        public IPromise Then(Action onResolved)
        {
            return Then(onResolved, null, null);
        }

        public IPromise Then(Action onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }

        public IPromise Then(Action onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    onResolved();
                    return this;
                }
                catch (Exception ex)
                {
                    return Rejected(ex);
                }
            }

            var resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            Action resolveHandler;
            if (onResolved != null)
            {
                resolveHandler = () =>
                {
                    onResolved();
                    resultPromise.Resolve();
                };
            }
            else
            {
                resolveHandler = resultPromise.Resolve;
            }

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = (ex) =>
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

        public IPromise Then(Func<IPromise> onResolved)
        {
            return Then(onResolved, null, null);
        }

        public IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }

        public IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved();
                }
                catch (Exception ex)
                {
                    return Rejected(ex);
                }
            }

            var resultPromise = GetRawPromise();
            resultPromise.WithName(Name);

            Action resolveHandler;
            if (onResolved != null)
            {
                resolveHandler = () =>
                {
                    onResolved()
                        .Progress(progress => resultPromise.ReportProgress(progress))
                        .Then(
                            () => resultPromise.Resolve(),
                            ex => resultPromise.Reject(ex)
                        );
                };
            }
            else
            {
                resolveHandler = resultPromise.Resolve;
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

        public IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved)
        {
            return this.Then<ConvertedT>(onResolved, null, null);
        }

        public IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved, Func<Exception, IPromise<ConvertedT>> onRejected, Action<float> onProgress)
        {
            // This version of the function must supply an onResolved.
            // Otherwise there is now way to get the converted value to pass to the resulting promise.
            Assert.IsNotNull(onResolved);

            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved();
                }
                catch (Exception ex)
                {
                    return Promise<ConvertedT>.Rejected(ex);
                }
            }

            var resultPromise = GetRawPromise<ConvertedT>();
            resultPromise.WithName(Name);

            Action resolveHandler = () => onResolved()
                               .Progress(progress => resultPromise.ReportProgress(progress))
                               .Then(
                                   (chainValue) => resultPromise.Resolve(chainValue),
                                   (ex) => resultPromise.Reject(ex)
                               );

            Action<Exception> rejectHandler = (ex) =>
            {
                if (onRejected == null)
                {
                    resultPromise.Reject(ex);
                    return;
                }

                try
                {
                    onRejected(ex).Then(
                        (chainValue) => resultPromise.Resolve(chainValue),
                        (callbackEx) => resultPromise.Reject(callbackEx)
                        );
                }
                catch (System.Exception callbackEx)
                {
                    resultPromise.Reject(callbackEx);
                }
            };

            ActionHandlers(resultPromise, resolveHandler, rejectHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        public IPromise ThenAll(Func<IEnumerable<IPromise>> chain)
        {
            return Then(() => AllInternal(chain()));
        }

        public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(() => Promise<ConvertedT>.All(chain()));
        }

        public IPromise ThenAny(Func<IEnumerable<IPromise>> chain)
        {
            return Then(() => AnyInternal(chain()));
        }

        public IPromise<ConvertedT> ThenAny<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(() => Promise<ConvertedT>.Any(chain()));
        }

        public IPromise ThenSequence(Func<IEnumerable<Func<IPromise>>> chain)
        {
            return Then(() => SequenceInternal(chain()));
        }

        public IPromise ThenRace(Func<IEnumerable<IPromise>> chain)
        {
            return Then(() => RaceInternal(chain()));
        }

        public IPromise<ConvertedT> ThenRace<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(() => Promise<ConvertedT>.Race(chain()));
        }

        public IPromise Finally(Action onComplete)
        {
            Assert.IsNotNull(onComplete);

            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    onComplete();
                    return this;
                }
                catch (Exception ex)
                {
                    return Rejected(ex);
                }
            }

            var promise = GetRawPromise();
            promise.WithName(Name);

            this.Then(promise.Resolve);
            this.Catch(e =>
            {
                // Things different from continue with
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

            return promise.Then(onComplete);
        }

        public IPromise ContinueWith(Func<IPromise> onComplete)
        {
            var promise = GetRawPromise();
            promise.WithName(Name);

            this.Then(promise.Resolve);
            this.Catch(e => promise.Resolve());
            return promise.Then(onComplete);
        }

        public IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete)
        {
            var promise = GetRawPromise();
            promise.WithName(Name);

            this.Then(promise.Resolve);
            this.Catch(e => promise.Resolve());
            return promise.Then(onComplete);
        }

        public IPromise Progress(Action<float> onProgress)
        {
            if (CurState == PromiseState.Pending && onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }
            return this;
        }

        #endregion

        #region IPendingPromise

        public void Resolve()
        {
            if (CurState != PromiseState.Pending)
            {
                throw new PromiseException(
                    "Attempt to resolve a promise that is already in state: " + CurState
                    + ", a promise can only be resolved when it is still in state: "
                    + PromiseState.Pending, PromiseExceptionType.Valid_STATE);
            }

            CurState = PromiseState.Resolved;
            if (EnablePromiseTracking)
            {
                PendingPromises.Remove(this);
            }

            InvokeResolveHandlers();
        }

        public void ResolveWrap<ConvertedT>(ConvertedT param)
        {
            Resolve();
        }

        public void ReportProgress(float progress)
        {
            if (CurState != PromiseState.Pending)
            {
                throw new PromiseException(
                    "Attempt to report progress on a promise that is already in state: "
                    + CurState + ", a promise can only report progress when it is still in state: "
                    + PromiseState.Pending,
                    PromiseExceptionType.Valid_STATE
                );
            }

            InvokeProgressHandlers(progress);
        }

        public void Reject(Exception ex)
        {
            Assert.NotNull(ex);
            if (CurState != PromiseState.Pending)
            {
                throw new PromiseException(
                    "Attempt to resolve a promise that is already in state: " + CurState
                    + ", a promise can only be resolved when it is still in state: "
                    + PromiseState.Pending, PromiseExceptionType.Valid_STATE);
            }

            rejectionException = ex;
            CurState = PromiseState.Rejected;

            if (EnablePromiseTracking)
            {
                PendingPromises.Remove(this);
            }

            InvokeRejectHandlers(ex);
        }

        #endregion

        #region private methods

        // Helper Function to invoke or register resolve/reject handlers
        protected void ActionHandlers(IRejectable resultPromise, Action resolveHandler, Action<Exception> rejectHandler)
        {
            switch (CurState)
            {
                case PromiseState.Resolved:
                    InvokeResolveHandler(resolveHandler, resultPromise);
                    break;
                case PromiseState.Rejected:
                    InvokeRejectHandler(rejectHandler, resultPromise, rejectionException);
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

        private void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
        {
            if (rejectHandlers == null)
            {
                rejectHandlers = new List<RejectHandler>();
            }

            rejectHandlers.Add(new RejectHandler
            {
                callback = onRejected,
                rejectable = rejectable
            });
        }

        private void AddResolveHandler(Action onResolved, IRejectable rejectable)
        {
            if (resolveHandlers == null)
            {
                resolveHandlers = new List<ResolveHandler>();
            }

            resolveHandlers.Add(new ResolveHandler
            {
                callback = onResolved,
                rejectable = rejectable
            });
        }

        private void AddProgressHandler(Action<float> onProgress, IRejectable rejectable)
        {
            if (progressHandlers == null)
            {
                progressHandlers = new List<ProgressHandler>();
            }

            progressHandlers.Add(new ProgressHandler
            {
                callback = onProgress,
                rejectable = rejectable
            });
        }

        // Invoke all reject handlers.
        private void InvokeRejectHandlers(Exception ex)
        {
            if (rejectHandlers != null)
            {
                for (int i = 0; i < rejectHandlers.Count; i++)
                {
                    RejectHandler handler = rejectHandlers[i];
                    InvokeRejectHandler(handler.callback, handler.rejectable, ex);
                }
            }

            ClearHandlers();
        }

        //Invoke all progress handlers.
        private void InvokeProgressHandlers(float progress)
        {
            if (progressHandlers != null)
            {
                for (int i = 0; i < progressHandlers.Count; i++)
                {
                    ProgressHandler handler = progressHandlers[i];
                    InvokeProgressHandler(handler.callback, handler.rejectable, progress);
                }
            }
        }

        // Invoke all resolve handlers
        private void InvokeResolveHandlers()
        {
            if (resolveHandlers != null)
            {
                for (int i = 0; i < resolveHandlers.Count; i++)
                {
                    ResolveHandler handler = resolveHandlers[i];
                    InvokeResolveHandler(handler.callback, handler.rejectable);
                }
            }

            ClearHandlers();
        }

        private void ClearHandlers()
        {
            rejectHandlers = null;
            resolveHandlers = null;
            progressHandlers = null;
        }

        private void InvokeResolveHandler(Action callback, IRejectable rejectable)
        {
            Assert.NotNull(callback);
            Assert.NotNull(rejectable);

            try
            {
                callback();
            }
            catch (Exception ex)
            {
                rejectable.Reject(ex);
            }
        }

        private void InvokeRejectHandler(Action<Exception> callback, IRejectable rejectable, Exception value)
        {
            Assert.NotNull(callback);
            Assert.NotNull(rejectable);

            try
            {
                callback(value);
            }
            catch (Exception ex)
            {
                rejectable.Reject(ex);
            }
        }

        private void InvokeProgressHandler(Action<float> callback, IRejectable rejectable, float progress)
        {
            Assert.NotNull(callback);
            Assert.NotNull(rejectable);

            try
            {
                callback(progress);
            }
            catch (Exception ex)
            {
                rejectable.Reject(ex);
                throw;
            }
        }



        #endregion

        #region static method

        // Convert an exception directly into a rejected promise.
        public static IPromise Rejected(Exception ex)
        {
            Assert.NotNull(ex);

            var promise = new Promise(PromiseState.Rejected);
            promise.rejectionException = ex;
            return promise;
        }

        public static IPromise Resolved()
        {
            return resolvedPromise;
        }

        // Raises the UnhandledException event.
        internal static void PropagateUnhandledException(object sender, Exception ex)
        {
            if (unhandlerException != null)
            {
                unhandlerException(sender, new ExceptionEventArgs(ex));
            }
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        public static IPromise Race(IEnumerable<IPromise> promises)
        {
            return resolvedPromise.RaceInternal(promises);
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        public static IPromise Race(params IPromise[] promises)
        {
            return resolvedPromise.RaceInternal(promises); // Cast is required to force use of the other function.
        }

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise All(params IPromise[] promises)
        {
            return resolvedPromise.AllInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise All(IEnumerable<IPromise> promises)
        {
            return resolvedPromise.AllInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        /// otherwise, it will be rejected  when all of them are rejected
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise Any(params IPromise[] promises)
        {
            return resolvedPromise.AnyInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        /// otherwise, it will be rejected  when all of them are rejected
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise Any(IEnumerable<IPromise> promises)
        {
            return resolvedPromise.AnyInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }
        /// <summary>
        /// Chain a number of operations using promises.
        /// Takes a number of functions each of which starts an async operation and yields a promise.
        /// </summary>
        public static IPromise Sequence(params Func<IPromise>[] fns)
        {
            return resolvedPromise.SequenceInternal((IEnumerable<Func<IPromise>>)fns);
        }

        /// <summary>
        /// Chain a sequence of operations using promises.
        /// Takes a collection of functions each of which starts an async operation and yields a promise.
        /// </summary>
        public static IPromise Sequence(IEnumerable<Func<IPromise>> fns)
        {
            return resolvedPromise.SequenceInternal((IEnumerable<Func<IPromise>>)fns);
        }

        #endregion

        #region private extension method

        protected virtual Promise<ConvertedT> GetRawPromise<ConvertedT>()
        {
            return new Promise<ConvertedT>();
        }

        protected virtual Promise GetRawPromise()
        {
            return new Promise();
        }


        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise AllInternal(params IPromise[] promises)
        {
            return AllInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        /// otherwise, it will be rejected  when all of them are rejected
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise AnyInternal(params IPromise[] promises)
        {
            return AnyInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when any of the promises in the enumerable argument have resolved.
        /// otherwise, it will be rejected  when all of them are rejected
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise AnyInternal(IEnumerable<IPromise> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                throw new PromiseException(
                    "At least 1 input promise must be provided for any",
                    PromiseExceptionType.EMPTY_PROMISE_ANY
                );
            }

            var remainingCount = promisesArray.Length;
            var progress = new float[remainingCount];
            var groupException = new PromiseGroupException(remainingCount);
            var resultPromise = GetRawPromise();
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
                    .Then(() =>
                    {
                        progress[index] = 1f;
                        resultPromise.ReportProgress(1f);

                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // return first fulfill promise
                            resultPromise.Resolve();
                        }
                    })
                    .Catch(ex =>
                    {
                        --remainingCount;
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
        /// Chain a number of operations using promises.
        /// Takes a number of functions each of which starts an async operation and yields a promise.
        /// </summary>
        protected IPromise SequenceInternal(params Func<IPromise>[] fns)
        {
            return SequenceInternal((IEnumerable<Func<IPromise>>)fns);
        }

        /// <summary>
        /// Chain a sequence of operations using promises.
        /// Takes a collection of functions each of which starts an async operation and yields a promise.
        /// </summary>
        protected IPromise SequenceInternal(IEnumerable<Func<IPromise>> fns)
        {
            var resultPromise = GetRawPromise();

            int count = 0;
            fns.Aggregate(
                Resolved(),
                (prevPromise, fn) =>
                {
                    int itemSequenceCount = count++;

                    return prevPromise
                        .Then(() =>
                        {
                            float sliceLength = 1 / (float)count;
                            resultPromise.ReportProgress(itemSequenceCount * sliceLength);
                            return fn();
                        })
                        .Progress((v) =>
                        {
                            float sliceLength = 1 / (float)count;
                            // v is curProgressValue 
                            // the evaluation of the formula below is
                            // ( all accumulated value + cur progress value ) * sliceRatio
                            resultPromise.ReportProgress((itemSequenceCount + v) * sliceLength);
                        });
                }
            )
            .Then(resultPromise.Resolve)
            .Catch(resultPromise.Reject);

            return resultPromise;
        }

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        protected IPromise AllInternal(IEnumerable<IPromise> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                return Resolved();
            }
            var remainingCount = promisesArray.Length;
            var resultPromise = GetRawPromise();
            var progress = new float[remainingCount];
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
            {
                promise
                    .Progress((v) =>
                    {
                        progress[index] = v;
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.ReportProgress(progress.Average());
                        }
                    })
                    .Then(() =>
                    {
                        progress[index] = 1f;

                        remainingCount--;
                        if (remainingCount == 0)
                        {
                            if (remainingCount <= 0 && resultPromise.CurState == PromiseState.Pending)
                            {
                                // This will never happen if any of the promises has en error occurred 
                                resultPromise.Resolve();
                            }
                        }
                        return resultPromise;
                    })
                    .Catch((ex) =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // If a promise has en error occurred 
                            // and the result promise is still pending, first reject it.
                            resultPromise.Reject(ex);
                        }
                    });
            });

            return resultPromise;
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        protected IPromise RaceInternal(params IPromise[] promises)
        {
            return RaceInternal((IEnumerable<IPromise>)promises); // Cast is required to force use of the other function.
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        protected IPromise RaceInternal(IEnumerable<IPromise> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                throw new PromiseException(
                    "At least 1 input promise must be provided for Race",
                    PromiseExceptionType.EMPTY_PROMISE_RACE
                );
            }
            var remainingCount = promisesArray.Length;
            var resultPromise = GetRawPromise();
            var progress = new float[remainingCount];
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
            {
                promise
                    .Progress((v) =>
                    {
                        progress[index] = v;
                        resultPromise.ReportProgress(progress.Max());
                    })
                    .Then(() =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.Resolve();
                        }
                    })
                    .Catch((ex) =>
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

        #endregion

    }

    /// <summary>
    /// Represents a handler invoked when the promise is resolved.
    /// </summary>
    public struct ResolveHandler
    {
        /// <summary>
        /// Callback fn.
        /// </summary>
        public Action callback;

        /// <summary>
        /// The promise that is rejected when there is an error while invoking the handler.
        /// </summary>
        public IRejectable rejectable;
    }

    /// <summary>
    /// Represents a handler invoked when the promise is rejected.
    /// </summary>
    public struct RejectHandler
    {
        /// <summary>
        /// Callback fn.
        /// </summary>
        public Action<Exception> callback;

        /// <summary>
        /// The promise that is rejected when there is an error while invoking the handler.
        /// </summary>
        public IRejectable rejectable;
    }

    public struct ProgressHandler
    {
        /// <summary>
        /// Callback fn.
        /// </summary>
        public Action<float> callback;

        /// <summary>
        /// The promise that is rejected when there is an error while invoking the handler.
        /// </summary>
        public IRejectable rejectable;
    }

}