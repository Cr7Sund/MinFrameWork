using NUnit.Framework;
using System;
using System.Linq;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public class Promise_NonGeneric_Tests
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Init(new InternalLogger());
        }
        
        [Test]
        public void can_resolve_simple_promise()
        {
            var promise = Promise.Resolved();

            int completed = 0;
            promise.Then(() => ++completed);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_simple_promise()
        {
            LogAssert.ignoreFailingMessages = true;

            var ex = new Exception();
            var promise = Promise.Rejected(ex);

            int errors = 0;
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                ++errors;
            });

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void exception_is_thrown_for_reject_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            promise.Reject(new Exception());

            var promiseException = Assert.Throws<MyException>(() => promise.Reject(new Exception()));
            Assert.AreEqual(PromiseExceptionType.Valid_REJECTED_STATE, promiseException.Type);
        }

        [Test]
        public void exception_is_thrown_for_reject_after_resolve()
        {
            var promise = new Promise();

            promise.Resolve();

            var promiseException = Assert.Throws<MyException>(() => promise.Reject(new Exception()));
            Assert.AreEqual(PromiseExceptionType.Valid_REJECTED_STATE, promiseException.Type);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            promise.Reject(new Exception());

            var promiseException = Assert.Throws<MyException>(() => promise.Resolve());
            Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
        }

        [Test]
        public void can_resolve_promise_and_trigger_then_handler()
        {
            var promise = new Promise();

            int completed = 0;

            promise.Then(() => ++completed);

            promise.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_resolve()
        {
            var promise = new Promise();

            promise.Resolve();

            var promiseException = Assert.Throws<MyException>(() => promise.Resolve());
            Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
        }

        [Test]
        public void can_resolve_promise_and_trigger_multiple_then_handlers_in_order()
        {
            var promise = new Promise();

            int completed = 0;

            promise.Then(() => Assert.AreEqual(1, ++completed));
            promise.Then(() => Assert.AreEqual(2, ++completed));

            promise.Resolve();

            Assert.AreEqual(2, completed);
        }

        [Test]
        public void can_resolve_promise_and_trigger_then_handler_with_callback_registration_after_resolve()
        {
            var promise = new Promise();

            int completed = 0;

            promise.Resolve();

            promise.Then(() => ++completed);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_promise_and_trigger_error_handler()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            var ex = new Exception();
            int completed = 0;
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                ++completed;
            });

            promise.Reject(ex);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_promise_and_trigger_multiple_error_handlers_in_order()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            var ex = new Exception();
            int completed = 0;

            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                Assert.AreEqual(1, ++completed);
            });
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                Assert.AreEqual(2, ++completed);
            });

            promise.Reject(ex);

            Assert.AreEqual(2, completed);
        }

        [Test]
        public void can_reject_promise_and_trigger_error_handler_with_registration_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            var ex = new Exception();
            promise.Reject(ex);

            int completed = 0;
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void error_handler_is_not_invoked_for_resolved_promised()
        {
            var promise = new Promise();

            promise.Catch(e => throw new Exception("This shouldn't happen"));

            promise.Resolve();
        }

        [Test]
        public void then_handler_is_not_invoked_for_rejected_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            promise.Then(() => throw new Exception("This shouldn't happen"));

            promise.Reject(new Exception("Rejection!"));
        }

        [Test]
        public void chain_multiple_promises_using_all()
        {
            var promise = new Promise();
            var chainedPromise1 = new Promise();
            var chainedPromise2 = new Promise();

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(() => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(() => ++completed);

                Assert.AreEqual(0, completed);

                promise.Resolve();

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve();

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve();

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void chain_multiple_promises_using_all_that_are_resolved_out_of_order()
        {
            var promise = new Promise();
            var chainedPromise1 = new Promise<int>();
            var chainedPromise2 = new Promise<int>();
            const int chainedResult1 = 10;
            const int chainedResult2 = 15;

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(() => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(result =>
                    {
                        int[] items = result.ToArray();
                        Assert.AreEqual(2, items.Length);
                        Assert.AreEqual(chainedResult1, items[0]);
                        Assert.AreEqual(chainedResult2, items[1]);

                        ++completed;
                    });

                Assert.AreEqual(0, completed);

                promise.Resolve();

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve(chainedResult1);

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve(chainedResult2);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void chain_multiple_value_promises_using_all_resolved_out_of_order()
        {
            var promise = new Promise();
            var chainedPromise1 = new Promise<int>();
            var chainedPromise2 = new Promise<int>();
            const int chainedResult1 = 10;
            const int chainedResult2 = 15;

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(() => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(result =>
                    {
                        int[] items = result.ToArray();
                        Assert.AreEqual(2, items.Length);
                        Assert.AreEqual(chainedResult1, items[0]);
                        Assert.AreEqual(chainedResult2, items[1]);

                        ++completed;
                    });

                Assert.AreEqual(0, completed);

                promise.Resolve();

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve(chainedResult2);

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve(chainedResult1);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_resolved_when_children_are_resolved()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(EnumerableExt.FromItems<IPromise>(promise1, promise2));

                int completed = 0;

                all.Then(() => ++completed);

                promise1.Resolve();
                promise2.Resolve();

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_first_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(EnumerableExt.FromItems<IPromise>(promise1, promise2));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Reject(new Exception("Error!"));
                promise2.Resolve();

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_second_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(EnumerableExt.FromItems<IPromise>(promise1, promise2));

                int errors = 0;
                all.Catch(e => { ++errors; });

                promise1.Resolve();
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_both_promises_are_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(EnumerableExt.FromItems<IPromise>(promise1, promise2));

                int errors = 0;
                var exception = new Exception("First Error");
                all.Catch(e =>
                {
                    Assert.AreEqual(exception, e);
                    ++errors;
                });

                promise1.Reject(exception);
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_resolved_if_there_are_no_promises()
        {
            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(Enumerable.Empty<IPromise>());

                int completed = 0;

                all.Then(() => ++completed);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_resolved_when_all_promises_are_already_resolved()
        {
            var promise1 = Promise.Resolved();
            var promise2 = Promise.Resolved();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise.All(EnumerableExt.FromItems(promise1, promise2));

                int completed = 0;

                all.Then(() => ++completed);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void all_with_rejected_promise()
        {
            LogAssert.ignoreFailingMessages = true;

            bool resolved = false;
            bool rejected = false;
            Exception caughtException = null;
            var exception = new Exception();

            var promiseA = new Promise();
            var promise = Promise
                .All(promiseA, Promise.Rejected(exception))
                .Then(() => resolved = true)
                .Catch(ex =>
                {
                    caughtException = ex;
                    rejected = true;
                });
            promiseA.ReportProgress(0.5f);
            promiseA.Resolve();

            Assert.AreEqual(false, resolved);
            Assert.AreEqual(true, rejected);
            Assert.AreEqual(exception, caughtException);
        }

        [Test]
        public void exception_thrown_in_all_continue_following_operations_from_being_invoked()
        {
            LogAssert.ignoreFailingMessages = true;
            int completed = 0;
            int result = 0;

            var promiseA = new Promise();
            var promiseB = new Promise();
            var promiseC = new Promise();
            promiseA.Then(() => ++completed);
            promiseB.Then(() => ++completed);
            promiseC.Then(() => ++completed);
            Promise
                .All(promiseA, promiseB, promiseC)
                .Then(() =>
                {
                    result = 100;
                })
                .Catch(_ => result = 10);
            promiseA.Resolve();
            promiseB.Reject(new Exception());
            promiseC.Resolve();

            Assert.AreEqual(2, completed);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void exception_thrown_during_transform_rejects_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            int errors = 0;
            var ex = new Exception();

            promise
                .Then(() => throw ex)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Resolve();

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void can_chain_promise()
        {
            var promise = new Promise();
            var chainedPromise = new Promise();

            int completed = 0;

            promise
                .Then(() => chainedPromise)
                .Then(() => ++completed);

            promise.Resolve();
            chainedPromise.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_chain_promise_and_convert_to_promise_that_yields_a_value()
        {
            var promise = new Promise();
            var chainedPromise = new Promise<string>();
            const string chainedPromiseValue = "some-value";

            int completed = 0;

            promise
                .Then(() => chainedPromise)
                .Then(v =>
                {
                    Assert.AreEqual(chainedPromiseValue, v);

                    ++completed;
                });
            promise.Resolve();
            chainedPromise.Resolve(chainedPromiseValue);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_in_chain_rejects_resulting_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();

            var ex = new Exception();
            int errors = 0;

            promise
                .Then(() => throw ex)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Resolve();

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void rejection_of_source_promise_rejects_chained_promise()
        {
            LogAssert.ignoreFailingMessages = true; 
            var promise = new Promise();
            var chainedPromise = new Promise();

            var ex = new Exception();
            int errors = 0;

            promise
                .Then(() => chainedPromise)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Reject(ex);

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void any_is_resolved_when_first_promise_is_resolved_first()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            var promise = Promise.Resolved();
            promise.ThenAny(() => new[]
                {
                    promise1, promise2
                })
                .Then(() => completed++);

            promise1.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void any_is_resolved_when_second_promise_is_resolved_first()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            var promise = Promise.Resolved();
            promise.ThenAny(() => new[]
                {
                    promise1, promise2
                })
                .Then(() => completed++);

            promise2.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void then_any_is_resolved_when_all_promise_resolved()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            var promise = Promise.Resolved();
            promise.ThenAny(() => new[]
                {
                    promise1, promise2
                })
                .Then(() => completed++);

            promise1.Resolve();
            promise2.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void any_is_resolved_when_second_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            var promise = Promise.Resolved();
            promise.ThenAny(() => new[]
                {
                    promise1, promise2
                })
                .Then(() => completed++);

            promise2.Reject(new Exception());
            promise1.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise
                    .Race(promise1, promise2)
                    .Then(() => ++completed);

                promise1.Resolve();

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void race_is_resolved_when_second_promise_is_resolved_first()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise
                    .Race(promise1, promise2)
                    .Then(() => ++completed);

                promise2.Resolve();

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void race_is_rejected_when_first_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            Exception ex = null;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise
                    .Race(promise1, promise2)
                    .Catch(e => ex = e);

                var expected = new Exception();
                promise1.Reject(expected);

                Assert.AreEqual(expected, ex);
            });
        }

        [Test]
        public void race_is_rejected_when_second_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise();
            var promise2 = new Promise();

            Exception ex = null;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise
                    .Race(promise1, promise2)
                    .Catch(e => ex = e);

                var expected = new Exception();
                promise2.Reject(expected);

                Assert.AreEqual(expected, ex);
            });
        }

        [Test]
        public void sequence_with_no_operations_is_directly_resolved()
        {
            int completed = 0;

            Promise
                .Sequence()
                .Then(() => ++completed);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequenced_is_not_resolved_when_operation_is_not_resolved()
        {
            int completed = 0;

            Promise
                .Sequence(() => new Promise())
                .Then(() => ++completed);

            Assert.AreEqual(0, completed);
        }

        [Test]
        public void sequence_is_resolved_when_operation_is_resolved()
        {
            int completed = 0;

            Promise
                .Sequence(Promise.Resolved)
                .Then(() => ++completed);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequence_is_unresolved_when_some_operations_are_unresolved()
        {
            int completed = 0;

            Promise
                .Sequence(
                    Promise.Resolved,
                    () => new Promise()
                )
                .Then(() => ++completed);

            Assert.AreEqual(0, completed);
        }

        [Test]
        public void sequence_is_resolved_when_all_operations_are_resolved()
        {
            int completed = 0;

            Promise
                .Sequence(Promise.Resolved, Promise.Resolved)
                .Then(() => ++completed);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequenced_operations_are_run_in_order_is_directly_resolved()
        {
            int order = 0;

            Promise
                .Sequence(
                    () =>
                    {
                        Assert.AreEqual(1, ++order);
                        return Promise.Resolved();
                    },
                    () =>
                    {
                        Assert.AreEqual(2, ++order);
                        return Promise.Resolved();
                    },
                    () =>
                    {
                        Assert.AreEqual(3, ++order);
                        return Promise.Resolved();
                    }
                );

            Assert.AreEqual(3, order);
        }

        [Test]
        // Compare to all_operations_are_run_in_order_is_parallel_resolved, 
        // show the difference between all and sequence
        public void sequenced_operations_are_run_in_order_is_parallel_resolved()
        {
            int order = 0;

            var promiseA = new Promise();
            var promiseB = new Promise();
            var promiseC = new Promise();

            Promise.Sequence(
                () =>
                {
                    return promiseA.Then(() =>
                    {
                        order += 1;
                        Assert.AreEqual(1, order);
                    });
                },
                () =>
                {
                    return promiseB.Then(() =>
                    {
                        order += 2;
                        Assert.AreEqual(3, order);
                    });
                },
                () =>
                {

                    return promiseC.Then(() =>
                    {
                        order += 3;
                        Assert.AreEqual(6, order);
                    });
                }
            );

            promiseB.Resolve();
            Assert.AreEqual(0, order);
            promiseC.Resolve();
            Assert.AreEqual(0, order);
            promiseA.Resolve();
            Assert.AreEqual(6, order);

        }

        [Test]
        // Compare to sequenced_operations_are_run_in_order_is_parallel_resolved, 
        // show the difference between all and sequence
        public void all_operations_are_run_in_order_is_parallel_resolved()
        {
            int order = 0;

            var promiseA = new Promise();
            var promiseB = new Promise();
            var promiseC = new Promise();

            Promise.All(
                promiseA.Then(() =>
                {
                    order += 1;
                    Assert.AreEqual(6, order);
                }),
                promiseB.Then(() =>
                {
                    order += 2;
                    Assert.AreEqual(2, order);
                }),
                promiseC.Then(() =>
                {
                    order += 3;
                    Assert.AreEqual(5, order);
                })
            );

            promiseB.Resolve();
            Assert.AreEqual(2, order);
            promiseC.Resolve();
            Assert.AreEqual(5, order);
            promiseA.Resolve();
            Assert.AreEqual(6, order);
        }


        [Test]
        public void exception_thrown_in_sequence_rejects_the_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            int errored = 0;
            int completed = 0;
            var ex = new Exception();

            Promise
                .Sequence(() => throw ex)
                .Then(() => ++completed)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);
                    ++errored;
                });

            Assert.AreEqual(1, errored);
            Assert.AreEqual(0, completed);
        }

        [Test]
        public void exception_thrown_in_sequence_stops_following_operations_from_being_invoked()
        {
            LogAssert.ignoreFailingMessages = true;
            int completed = 0;

            Promise
                .Sequence(
                    () =>
                    {
                        ++completed;
                        return Promise.Resolved();
                    },
                    () => throw new Exception(),
                    () =>
                    {
                        ++completed;
                        return Promise.Resolved();
                    }
                );

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_resolve_promise_via_resolver_function()
        {
            var promise = new Promise((resolve, reject) =>
            {
                resolve();
            });

            int completed = 0;
            promise.Then(() =>
            {
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_promise_via_reject_function()
        {
            LogAssert.ignoreFailingMessages = true;
            var ex = new Exception();
            var promise = new Promise((resolve, reject) =>
            {
                reject(ex);
            });

            int completed = 0;
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_during_resolver_rejects_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var ex = new Exception();
            var promise = new Promise((resolve, reject) => throw ex);

            int completed = 0;
            promise.Catch(e =>
            {
                Assert.AreEqual(ex, e);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void unhandled_exception_is_propagated_via_event()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            var ex = new Exception();
            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) =>
            {
                Assert.AreEqual(ex, e.Exception);

                ++eventRaised;
            };

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Then(() => throw ex)
                    .Done();

                promise.Resolve();

                Assert.AreEqual(1, eventRaised);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }
        }

        [Test]
        public void exception_in_done_callback_is_propagated_via_event()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            var ex = new Exception();
            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) =>
            {
                Assert.AreEqual(ex, e.Exception);

                ++eventRaised;
            };

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Done(() => throw ex);

                promise.Resolve();

                Assert.AreEqual(1, eventRaised);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }
        }

        [Test]
        public void handled_exception_is_not_propagated_via_event()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            var ex = new Exception();
            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) => ++eventRaised;

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Then(() => throw ex)
                    .Catch(_ =>
                    {
                        // Catch the error.
                    })
                    .Done();

                promise.Resolve();

                Assert.AreEqual(0, eventRaised);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }

        }

        [Test]
        public void can_handle_Done_onResolved()
        {
            var promise = new Promise();
            int callback = 0;

            promise.Done(() => ++callback);

            promise.Resolve();

            Assert.AreEqual(1, callback);
        }

        [Test]
        public void can_handle_Done_onResolved_with_onReject()
        {
            var promise = new Promise();
            int callback = 0;
            int errorCallback = 0;

            promise.Done(
                () => ++callback,
                ex => ++errorCallback
            );

            promise.Resolve();

            Assert.AreEqual(1, callback);
            Assert.AreEqual(0, errorCallback);
        }

        //  * Also want a test that exception thrown during Then triggers the error handler.
        //  * How do Javascript promises work in this regard?
        // [Test]
        // public void exception_during_Done_onResolved_triggers_error_handler()
        // {
        //     var promise = new Promise<int>();
        //     var callback = 0;
        //     var errorCallback = 0;
        //     var expectedValue = 5;
        //     var expectedException = new Exception();

        //     promise.Done(
        //         value =>
        //         {
        //             Assert.AreEqual(expectedValue, value);

        //             ++callback;

        //             throw expectedException;
        //         },
        //         ex =>
        //         {
        //             Assert.AreEqual(expectedException, ex);

        //             ++errorCallback;
        //         }
        //     );

        //     promise.Resolve(expectedValue);

        //     Assert.AreEqual(1, callback);
        //     Assert.AreEqual(1, errorCallback);
        // }

        [Test]
        public void exception_during_Then_onResolved_triggers_error_handler()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;
            int errorCallback = 0;
            var expectedException = new Exception();

            promise
                .Then(() =>
                {
                    throw expectedException;
                })
                .Done(
                    () => ++callback,
                    ex =>
                    {
                        Assert.AreEqual(expectedException, ex);

                        ++errorCallback;
                    }
                );

            promise.Resolve();

            Assert.AreEqual(0, callback);
            Assert.AreEqual(1, errorCallback);
        }

        [Test]
        public void inner_exception_handled_by_outer_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int errorCallback = 0;
            var expectedException = new Exception();

            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) => ++eventRaised;

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Then(() => Promise.Resolved().Then(() => throw expectedException))
                    .Catch(ex =>
                    {
                        Assert.AreEqual(expectedException, ex);

                        ++errorCallback;
                    });

                promise.Resolve();

                // No "done" in the chain, no generic event handler should be called
                Assert.AreEqual(0, eventRaised);

                // Instead the catch should have got the exception
                Assert.AreEqual(1, errorCallback);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }
        }

        [Test]
        public void inner_exception_handled_by_outer_promise_with_results()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            int errorCallback = 0;
            var expectedException = new Exception();

            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) => ++eventRaised;

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Then(_ => Promise<int>.Resolved(5).Then(__ => throw expectedException))
                    .Catch(ex =>
                    {
                        Assert.AreEqual(expectedException, ex);

                        ++errorCallback;
                    });

                promise.Resolve(2);

                // No "done" in the chain, no generic event handler should be called
                Assert.AreEqual(0, eventRaised);

                // Instead the catch should have got the exception
                Assert.AreEqual(1, errorCallback);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }
        }

        [Test]
        public void promises_have_sequential_ids()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            Assert.AreEqual(promise1.Id + 1, promise2.Id);
        }


        [Test]
        public void finally_is_called_after_resolve()
        {
            var promise = new Promise();
            int callback = 0;

            promise.Finally(() => ++callback);

            promise.Resolve();

            Assert.AreEqual(1, callback);
        }

        [Test]
        public void finally_is_called_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;

            promise.Finally(() => ++callback);

            promise.Reject(new Exception());

            Assert.AreEqual(1, callback);
        }

        [Test]
        public void resolved_chain_continues_after_finally()
        {
            var promise = new Promise();
            int callback = 0;

            promise.Finally(() =>
                {
                    Assert.AreEqual(0, callback);
                    ++callback;
                })
                .Then(() => callback += 2);

            promise.Resolve();

            Assert.AreEqual(3, callback);
        }

        [Test]
        public void rejected_chain_rejects_after_finally()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;

            promise.Finally(() => ++callback)
                .Catch(_ => ++callback);

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void rejected_chain_continues_after_ContinueWith_returning_non_value_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;

            promise.ContinueWith(() =>
                {
                    ++callback;
                    return Promise.Resolved();
                })
                .Then(() => ++callback);

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void rejected_chain_continues_after_ContinueWith_returning_value_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;
            const string expectedValue = "foo";

            promise.ContinueWith(() =>
                {
                    ++callback;
                    return Promise<string>.Resolved("foo");
                })
                .Then(x =>
                {
                    Assert.AreEqual(expectedValue, x);
                    ++callback;
                });

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        //tc39 note: "a throw (or returning a rejected promise) in the finally callback will reject the new promise with that rejection reason."
        public void exception_in_finally_callback_is_caught_by_chained_catch()
        {
            //NOTE: Also tests that the new exception is passed thru promise chain
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;
            var expectedException = new Exception("Expected");

            promise.Finally(() =>
                {
                    ++callback;
                    throw expectedException;
                })
                .Catch(ex =>
                {
                    Assert.AreEqual(expectedException, ex);
                    ++callback;
                });

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void exception_in_ContinueWith_callback_returning_non_value_promise_is_caught_by_chained_catch()
        {
            LogAssert.ignoreFailingMessages = true;
            //NOTE: Also tests that the new exception is passed thru promise chain

            var promise = new Promise();
            int callback = 0;
            var expectedException = new Exception("Expected");

            promise.ContinueWith(() =>
                {
                    ++callback;
                    throw expectedException;
                })
                .Catch(ex =>
                {
                    Assert.AreEqual(expectedException, ex);
                    ++callback;
                });

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void exception_in_ContinueWith_callback_returning_value_promise_is_caught_by_chained_catch()
        {
            //NOTE: Also tests that the new exception is passed through promise chain
            
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise();
            int callback = 0;
            var expectedException = new Exception("Expected");

            promise.ContinueWith(new Func<IPromise<int>>(() =>
                {
                    ++callback;
                    throw expectedException;
                }))
                .Catch(ex =>
                {
                    Assert.AreEqual(expectedException, ex);
                    ++callback;
                });

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void can_chain_promise_after_ContinueWith()
        {
            var promise = new Promise();
            const int expectedValue = 5;
            int callback = 0;

            promise.ContinueWith(() =>
                {
                    ++callback;
                    return Promise<int>.Resolved(expectedValue);
                })
                .Then(x =>
                {
                    Assert.AreEqual(expectedValue, x);
                    ++callback;
                });

            promise.Resolve();

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void pending_count_resolve_outer_first()
        {
            LogAssert.ignoreFailingMessages = true;
            Promise.EnablePromiseTracking = true;
            Promise.ClearPending();

            {
                var promise = new Promise();
                var promise1 = new Promise();
                var promise2 = new Promise();
                var childPromise = new Promise();
                var valuePromise = new Promise<int>();
                Assert.AreEqual(5, Promise.GetPendingPromiseCount());

                promise.Then(() => childPromise);
                Assert.AreEqual(6, Promise.GetPendingPromiseCount());
                promise.Then(() => valuePromise);
                Assert.AreEqual(7, Promise.GetPendingPromiseCount());

                promise.Resolve();
                Assert.AreEqual(8, Promise.GetPendingPromiseCount());

                childPromise.Resolve();
                Assert.AreEqual(5, Promise.GetPendingPromiseCount());

                valuePromise.Resolve(2);
                Assert.AreEqual(2, Promise.GetPendingPromiseCount());

                promise1.Reject(new Exception());
                promise2.Reject(new Exception());
                Assert.AreEqual(0, Promise.GetPendingPromiseCount());
            }
            Promise.EnablePromiseTracking = false;
            Promise.ClearPending();

        }

        [Test]
        public void pending_count_resolve_inner_first()
        {
            LogAssert.ignoreFailingMessages = true;
            Promise.EnablePromiseTracking = true;
            Promise.ClearPending();

            {
                var promise = new Promise();
                var promise1 = new Promise();
                var promise2 = new Promise();
                var childPromise = new Promise();
                var valuePromise = new Promise<int>();
                Assert.AreEqual(5, Promise.GetPendingPromiseCount());
                promise.Then(() => childPromise);
                Assert.AreEqual(6, Promise.GetPendingPromiseCount());
                promise.Then(() => valuePromise);
                Assert.AreEqual(7, Promise.GetPendingPromiseCount());


                childPromise.Resolve();
                Assert.AreEqual(6, Promise.GetPendingPromiseCount());

                valuePromise.Resolve(2);
                Assert.AreEqual(5, Promise.GetPendingPromiseCount());

                promise.Resolve();
                Assert.AreEqual(2, Promise.GetPendingPromiseCount());

                promise1.Reject(new Exception());
                promise2.Reject(new Exception());
                Assert.AreEqual(0, Promise.GetPendingPromiseCount());
            }
            Promise.EnablePromiseTracking = false;
            Promise.ClearPending();

        }
    }
}
