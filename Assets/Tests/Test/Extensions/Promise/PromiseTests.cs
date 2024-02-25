using NUnit.Framework;
using System;
using System.Linq;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(new InternalLogger());
        }
        
        [Test]
        public void can_resolve_simple_promise()
        {
            const int promisedValue = 5;
            var promise = Promise<int>.Resolved(promisedValue);

            int completed = 0;
            promise.Then(v =>
            {
                Assert.AreEqual(promisedValue, v);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_simple_promise()
        {
            LogAssert.ignoreFailingMessages = true;

            var ex = new Exception();
            var promise = Promise<int>.Rejected(ex);

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
            var promise = new Promise<int>();

            promise.Reject(new Exception());

            var promiseException = Assert.Throws<MyException>(() => promise.Reject(new Exception()));
            Assert.AreEqual(PromiseExceptionType.Valid_REJECTED_STATE, promiseException.Type);
        }

        [Test]
        public void exception_is_thrown_for_reject_after_resolve()
        {
            var promise = new Promise<int>();

            promise.Resolve(5);

            var promiseException = Assert.Throws<MyException>(() => promise.Reject(new Exception()));
            Assert.AreEqual(PromiseExceptionType.Valid_REJECTED_STATE, promiseException.Type);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

            promise.Reject(new Exception());

            var promiseException = Assert.Throws<MyException>(() => promise.Resolve(5));
            Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
        }

        [Test]
        public void can_resolve_promise_and_trigger_then_handler()
        {
            var promise = new Promise<int>();

            int completed = 0;
            const int promisedValue = 15;

            promise.Then(v =>
            {
                Assert.AreEqual(promisedValue, v);
                ++completed;
            });

            promise.Resolve(promisedValue);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_resolve()
        {
            var promise = new Promise<int>();

            promise.Resolve(5);

            var promiseException = Assert.Throws<MyException>(() => promise.Resolve(5));
            Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
        }

        [Test]
        public void can_resolve_promise_and_trigger_multiple_then_handlers_in_order()
        {
            var promise = new Promise<int>();

            int completed = 0;

            promise.Then(v => Assert.AreEqual(1, ++completed));
            promise.Then(v => Assert.AreEqual(2, ++completed));

            promise.Resolve(1);

            Assert.AreEqual(2, completed);
        }


        [Test]
        public void can_resolve_promise_and_trigger_then_handler_with_callback_registration_after_resolve()
        {
            var promise = new Promise<int>();

            int completed = 0;
            const int promisedValue = -10;

            promise.Resolve(promisedValue);

            promise.Then(v =>
            {
                Assert.AreEqual(promisedValue, v);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_promise_and_trigger_error_handler()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

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
            var promise = new Promise<int>();

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
            var promise = new Promise<int>();

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
            var promise = new Promise<int>();

            promise.Catch(e => throw new Exception("This shouldn't happen"));

            promise.Resolve(5);
        }

        [Test]
        public void then_handler_is_not_invoked_for_rejected_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

            promise.Then(v => throw new Exception("This shouldn't happen"));

            promise.Reject(new Exception("Rejection!"));
        }

        [Test]
        public void chain_multiple_promises_using_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            var chainedPromise1 = Promise<int>.Rejected(new Exception());
            var chainedPromise2 = Promise<int>.Rejected(new Exception());
            var chainedPromise3 = Promise<int>.Resolved(9001);

            bool completed = false;

            Promise<int>
                .First(() => chainedPromise1, () => chainedPromise2, () => chainedPromise3, () =>
                {
                    Assert.True(false, "Didn't stop on the first resolved promise");
                    return Promise<int>.Rejected(new Exception());
                })
                .Then(result =>
                {
                    Assert.AreEqual(9001, result);
                    completed = true;
                })
                ;

            Assert.AreEqual(true, completed);
        }

        [Test]
        public void chain_multiple_rejected_promises_using_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            var chainedPromise1 = Promise<int>.Rejected(new Exception("First chained promise"));
            var chainedPromise2 = Promise<int>.Rejected(new Exception("Second chained promise"));
            var chainedPromise3 = Promise<int>.Rejected(new Exception("Third chained promise"));

            bool completed = false;

            Promise<int>
                .First(() => chainedPromise1, () => chainedPromise2, () => chainedPromise3)
                .Catch(ex =>
                {
                    Assert.AreEqual("Third chained promise", ex.Message);
                    completed = true;
                })
                ;

            Assert.AreEqual(true, completed);
        }

        [Test]
        public void chain_multiple_promises_using_then_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = Promise<int>.Resolved(20);
            var chainedPromise1 = new Promise<int>();
            chainedPromise1.Then(v => v += 1);
            var chainedPromise2 = new Promise<int>();
            chainedPromise2.Then(v => v += 2);
            var chainedPromise3 = new Promise<int>();
            chainedPromise3.Then(v => v += 3);

            bool isFail = false;
            bool isSuccessful = false;

            promise
                .ThenFirst(new Func<IPromise<int>>[]
                {
                    () =>
                    {
                        return Promise<int>.Rejected(new Exception());
                    },
                    () => chainedPromise1, () => chainedPromise2,
                    () => chainedPromise3
                })
                .Then(result =>
                {
                    Assert.AreEqual(10, result);
                    isSuccessful = true;
                })
                .Catch(ex =>
                {
                    isFail = true;
                });
            ;

            chainedPromise1.Resolve(10);
            chainedPromise2.Resolve(100);
            chainedPromise3.Resolve(1000);

            Assert.AreEqual(false, isFail);
            Assert.AreEqual(true, isSuccessful);
        }


        [Test]
        public void chain_multiple_promises_rejected_using_then_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = Promise<int>.Resolved(20);
            var chainedPromise1 = new Promise<int>();
            chainedPromise1.Then(v => v += 1);
            var chainedPromise2 = new Promise<int>();
            chainedPromise2.Then(v => v += 2);
            var chainedPromise3 = new Promise<int>();
            chainedPromise3.Then(v => v += 13);

            bool isFail = false;
            bool isSuccessful = false;
            promise
                .ThenFirst(new Func<IPromise<int>>[]
                {
                    () =>
                    {
                        return Promise<int>.Rejected(new Exception());
                    },
                    () => chainedPromise1, () => chainedPromise2,
                    () => chainedPromise3
                })
                .Then(result =>
                {
                    isSuccessful = true;
                })
                .Catch(ex =>
                {
                    isFail = true;
                });
            ;

            chainedPromise1.Resolve(1);
            chainedPromise2.Resolve(1);
            chainedPromise3.Resolve(1);

            Assert.AreEqual(true, isSuccessful);
            Assert.AreEqual(false, isFail);
        }

        [Test]
        public void chain_multiple_promises_using_all()
        {
            var promise = new Promise<string>();
            var chainedPromise1 = new Promise<int>();
            var chainedPromise2 = new Promise<int>();
            const int chainedResult1 = 10;
            const int chainedResult2 = 15;

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(i => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(result =>
                    {
                        int[] items = result.ToArray();
                        Assert.AreEqual(2, items.Length);
                        Assert.AreEqual(chainedResult1, items[0]);
                        Assert.AreEqual(chainedResult2, items[1]);

                        ++completed;
                    });

                Assert.AreEqual(0, completed);

                promise.Resolve("hello");

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve(chainedResult1);

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve(chainedResult2);

                Assert.AreEqual(1, completed);
            });
        }


        [Test]
        public void chain_multiple_promises_using_all_that_are_resolved_out_of_order()
        {
            var promise = new Promise<string>();
            var chainedPromise1 = new Promise<int>();
            var chainedPromise2 = new Promise<int>();
            const int chainedResult1 = 10;
            const int chainedResult2 = 15;

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(i => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(result =>
                    {
                        int[] items = result.ToArray();
                        Assert.AreEqual(2, items.Length);
                        Assert.AreEqual(chainedResult1, items[0]);
                        Assert.AreEqual(chainedResult2, items[1]);

                        ++completed;
                    });

                Assert.AreEqual(0, completed);

                promise.Resolve("hello");

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve(chainedResult2);

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve(chainedResult1);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void chain_multiple_promises_using_all_and_convert_to_non_value_promise()
        {
            var promise = new Promise<string>();
            var chainedPromise1 = new Promise();
            var chainedPromise2 = new Promise();

            int completed = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .ThenAll(i => EnumerableExt.FromItems(chainedPromise1, chainedPromise2))
                    .Then(() => ++completed);

                Assert.AreEqual(0, completed);

                promise.Resolve("hello");

                Assert.AreEqual(0, completed);

                chainedPromise1.Resolve();

                Assert.AreEqual(0, completed);

                chainedPromise2.Resolve();

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_resolved_when_children_are_resolved()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(EnumerableExt.FromItems<IPromise<int>>(promise1, promise2));

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    int[] values = v.ToArray();
                    Assert.AreEqual(2, values.Length);
                    Assert.AreEqual(1, values[0]);
                    Assert.AreEqual(2, values[1]);
                });

                promise1.Resolve(1);
                promise2.Resolve(2);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_of_multiple_types_is_resolved_when_children_are_resolved()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2);

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    Assert.AreEqual(1, v.Item1);
                    Assert.AreEqual(true, v.Item2);
                });

                promise1.Resolve(1);
                promise2.Resolve(true);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_of_three_types_is_resolved_when_children_are_resolved()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();
            var promise3 = new Promise<float>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2, promise3);

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    Assert.AreEqual(1, v.Item1);
                    Assert.AreEqual(true, v.Item2);
                    Assert.AreEqual(3.0f, v.Item3);
                });

                promise1.Resolve(1);
                promise2.Resolve(true);
                promise3.Resolve(3.0f);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_of_four_types_is_resolved_when_children_are_resolved()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();
            var promise3 = new Promise<float>();
            var promise4 = new Promise<double>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2, promise3, promise4);

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    Assert.AreEqual(1, v.Item1);
                    Assert.AreEqual(true, v.Item2);
                    Assert.AreEqual(3.0f, v.Item3);
                    Assert.AreEqual(4.0, v.Item4);
                });

                promise1.Resolve(1);
                promise2.Resolve(true);
                promise3.Resolve(3.0f);
                promise4.Resolve(4.0);

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_first_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(EnumerableExt.FromItems<IPromise<int>>(promise1, promise2));

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Reject(new Exception("Error!"));
                promise2.Resolve(2);

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_of_multiple_types_is_rejected_when_first_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2);

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Reject(new Exception("Error!"));
                promise2.Resolve(true);

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_second_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(EnumerableExt.FromItems<IPromise<int>>(promise1, promise2));

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Resolve(2);
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_of_multiple_types_is_rejected_when_second_promise_is_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2);

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Resolve(2);
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_rejected_when_both_promises_are_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(EnumerableExt.FromItems<IPromise<int>>(promise1, promise2));

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => { ++errors; });

                promise1.Reject(new Exception("Error!"));
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_of_multiple_types_is_rejected_when_both_promises_are_rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<bool>();

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2);

                all.Then(v => throw new Exception("Shouldn't happen"));

                int errors = 0;
                all.Catch(e => ++errors);

                promise1.Reject(new Exception("Error!"));
                promise2.Reject(new Exception("Error!"));

                Assert.AreEqual(1, errors);
            });
        }

        [Test]
        public void combined_promise_is_resolved_if_there_are_no_promises()
        {
            LogAssert.ignoreFailingMessages = true;
            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(Enumerable.Empty<IPromise<int>>());

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    CollectionAssert.IsEmpty(v);
                });

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_is_resolved_when_all_promises_are_already_resolved()
        {
            LogAssert.ignoreFailingMessages = true;

            var promise1 = Promise<int>.Resolved(1);
            var promise2 = Promise<int>.Resolved(1);

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = Promise<int>.All(EnumerableExt.FromItems(promise1, promise2));

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    CollectionAssert.IsEmpty(v);
                });

                Assert.AreEqual(1, completed);
            });
        }

        [Test]
        public void combined_promise_of_multiple_types_is_resolved_when_all_promises_are_already_resolved()
        {
            var promise1 = Promise<int>.Resolved(1);
            var promise2 = Promise<bool>.Resolved(true);

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                var all = PromiseHelpers.All(promise1, promise2);

                int completed = 0;

                all.Then(v =>
                {
                    ++completed;

                    Assert.AreEqual(1, v.Item1);
                    Assert.AreEqual(true, v.Item2);
                });

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

            var promiseA = new Promise<int>();
            var promise = Promise<int>
                .All(promiseA, Promise<int>.Rejected(exception))
                .Then(values => resolved = true)
                .Catch(ex =>
                {
                    caughtException = ex;
                    rejected = true;
                });
            promiseA.ReportProgress(0.5f);
            promiseA.Resolve(0);

            Assert.AreEqual(false, resolved);
            Assert.AreEqual(true, rejected);
            Assert.AreEqual(exception, caughtException);
        }

        [Test]
        public void can_transform_promise_value()
        {
            var promise = new Promise<int>();

            int promisedValue = 15;
            int completed = 0;

            promise
                .Then(v => v.ToString())
                .Then(v =>
                {
                    Assert.AreEqual(promisedValue.ToString(), v);

                    ++completed;
                });

            promise.Resolve(promisedValue);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void rejection_of_source_promise_rejects_transformed_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

            var ex = new Exception();
            int errors = 0;

            promise
                .Then(v => v.ToString())
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Reject(ex);

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void exception_thrown_during_transform_rejects_transformed_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

            const int promisedValue = 15;
            int errors = 0;
            var ex = new Exception();

            promise
                .Then(v => throw ex)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Resolve(promisedValue);

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void can_chain_promise_and_convert_type_of_value()
        {
            var promise = new Promise<int>();
            var chainedPromise = new Promise<string>();

            const int promisedValue = 15;
            const string chainedPromiseValue = "blah";
            int completed = 0;

            promise
                .Then<string>(v => chainedPromise)
                .Then(v =>
                {
                    Assert.AreEqual(chainedPromiseValue, v);

                    ++completed;
                });

            promise.Resolve(promisedValue);
            chainedPromise.Resolve(chainedPromiseValue);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_chain_promise_and_convert_to_non_value_promise()
        {
            var promise = new Promise<int>();
            var chainedPromise = new Promise();

            const int promisedValue = 15;
            int completed = 0;

            promise
                .Then(v => (IPromise)chainedPromise)
                .Then(() => ++completed);

            promise.Resolve(promisedValue);
            chainedPromise.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_in_chain_rejects_resulting_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();

            var ex = new Exception();
            int errors = 0;

            promise
                .Then(v => throw ex)
                .Catch(e =>
                {
                    Assert.AreEqual(ex, e);

                    ++errors;
                });

            promise.Resolve(15);

            Assert.AreEqual(1, errors);
        }

        [Test]
        public void rejection_of_source_promise_rejects_chained_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            var chainedPromise = new Promise<string>();

            var ex = new Exception();
            int errors = 0;

            promise
                .Then<string>(v => chainedPromise)
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
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Then(i => resolved = i);

                promise1.Resolve(5);

                Assert.AreEqual(5, resolved);
            });
        }

        [Test]
        public void any_is_resolved_when_second_promise_is_resolved_first()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Then(i => resolved = i);

                promise2.Resolve(12);

                Assert.AreEqual(12, resolved);
            });
        }

        [Test]
        public void any_is_resolved_when_all_promise_resolved()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Then(i => resolved = i);

                promise2.Resolve(12);
                promise1.Resolve(122);

                Assert.AreEqual(12, resolved);
            });
        }

        [Test]
        public void then_any_is_resolved_when_all_promise_resolved()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            var promise = Promise.Resolved();
            promise.ThenAny(() => new[]
                {
                    promise1, promise2
                })
                .Then(i => resolved = i);

            promise2.Resolve(12);
            promise1.Resolve(122);

            Assert.AreEqual(12, resolved);
        }


        [Test]
        public void any_is_rejected_when_all_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Catch(e => ex = e);

                var exception2 = new Exception();
                var exception1 = new Exception();
                promise1.Reject(exception1);
                promise2.Reject(exception2);

                Assert.AreEqual(typeof(PromiseGroupException), ex.GetType());
                Assert.AreEqual(exception1, ((PromiseGroupException)ex).Exceptions[0]);
                Assert.AreEqual(exception2, ((PromiseGroupException)ex).Exceptions[1]);
            });
        }

        [Test]
        public void any_is_rejected_when_first_promise_is_rejected_first_but_exist_Resolve()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;
            int expected = 2;
            int resolved = 4;
            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Then(v => resolved = expected)
                    .Catch(e => ex = e);

                var exception = new Exception();
                promise1.Reject(exception);
                promise2.Resolve(expected);

                Assert.IsNull(ex);
                Assert.AreEqual(expected, resolved);
            });
        }

        [Test]
        public void any_is_rejected_when_second_promise_is_rejected_next_but_exist_Resolve()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;
            int expected = 2;
            int resolved = 4;
            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Any(promise1, promise2)
                    .Then(v => resolved = expected)
                    .Catch(e => ex = e);


                var exception = new Exception();
                promise2.Reject(exception);
                promise1.Resolve(expected);

                Assert.IsNull(ex);
                Assert.AreEqual(expected, resolved);
            });
        }

        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Race(promise1, promise2)
                    .Then(i => resolved = i);

                promise1.Resolve(5);

                Assert.AreEqual(5, resolved);
            });
        }

        [Test]
        public void race_is_resolved_when_second_promise_is_resolved_first()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            int resolved = 0;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Race(promise1, promise2)
                    .Then(i => resolved = i);

                promise2.Resolve(12);

                Assert.AreEqual(12, resolved);
            });
        }

        [Test]
        public void race_is_rejected_when_first_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
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
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;

            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                Promise<int>
                    .Race(promise1, promise2)
                    .Catch(e => ex = e);

                var expected = new Exception();
                promise2.Reject(expected);

                Assert.AreEqual(expected, ex);
            });
        }

        [Test]
        public void then_race_is_rejected_when_second_promise_is_rejected_first()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Exception ex = null;

            var promise = Promise.Resolved();
            TestHelpers.VerifyDoesntThrowUnhandledException(() =>
            {
                promise
                    .Then(() => { })
                    .ThenRace(() => new[]
                    {
                        promise1, promise2
                    })
                    .Catch(e => ex = e);

                var expected = new Exception();
                promise2.Reject(expected);

                Assert.AreEqual(expected, ex);
            });
        }

        [Test]
        public void can_resolve_promise_via_resolver_function()
        {
            var promise = new Promise<int>((resolve, reject) => resolve(5));

            int completed = 0;
            promise.Then(v =>
            {
                Assert.AreEqual(5, v);
                ++completed;
            });

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_promise_via_reject_function()
        {
            LogAssert.ignoreFailingMessages = true;
            var ex = new Exception();
            var promise = new Promise<int>((resolve, reject) =>
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
            var promise = new Promise<int>((resolve, reject) => throw ex);

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
            var promise = new Promise<int>();
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
                    .Then(a => throw ex)
                    .Done();

                promise.Resolve(5);

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
            var promise = new Promise<int>();
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
                    .Done(x => throw ex);

                promise.Resolve(5);

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
            var promise = new Promise<int>();
            var ex = new Exception();
            int eventRaised = 0;

            EventHandler<ExceptionEventArgs> handler = (s, e) => ++eventRaised;

            Promise.UnhandledException += handler;

            try
            {
                promise
                    .Then(a => throw ex)
                    .Catch(_ =>
                    {
                        // Catch the error.
                    })
                    .Done();

                promise.Resolve(5);

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
            var promise = new Promise<int>();
            int callback = 0;
            const int expectedValue = 5;

            promise.Done(value =>
            {
                Assert.AreEqual(expectedValue, value);

                ++callback;
            });

            promise.Resolve(expectedValue);

            Assert.AreEqual(1, callback);
        }

        [Test]
        public void can_handle_Done_onResolved_with_onReject()
        {
            var promise = new Promise<int>();
            int callback = 0;
            int errorCallback = 0;
            const int expectedValue = 5;

            promise.Done(
                value =>
                {
                    Assert.AreEqual(expectedValue, value);

                    ++callback;
                },
                ex => ++errorCallback
            );

            promise.Resolve(expectedValue);

            Assert.AreEqual(1, callback);
            Assert.AreEqual(0, errorCallback);
        }

        /*todo:
         * Also want a test that exception thrown during Then triggers the error handler.
         * How do Javascript promises work in this regard?
        [Test]
        public void exception_during_Done_onResolved_triggers_error_handler()
        {
            var promise = new Promise<int>();
            var callback = 0;
            var errorCallback = 0;
            var expectedValue = 5;
            var expectedException = new Exception();

            promise.Done(
                value =>
                {
                    Assert.AreEqual(expectedValue, value);

                    ++callback;

                    throw expectedException;
                },
                ex =>
                {
                    Assert.AreEqual(expectedException, ex);

                    ++errorCallback;
                }
            );

            promise.Resolve(expectedValue);

            Assert.AreEqual(1, callback);
            Assert.AreEqual(1, errorCallback);
        }
         * */

        [Test]
        public void exception_during_Then_onResolved_triggers_error_handler()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            int callback = 0;
            int errorCallback = 0;
            var expectedException = new Exception();

            promise
                .Then(value => throw expectedException)
                .Done(
                    () => ++callback,
                    ex =>
                    {
                        Assert.AreEqual(expectedException, ex);

                        ++errorCallback;
                    }
                );

            promise.Resolve(6);

            Assert.AreEqual(0, callback);
            Assert.AreEqual(1, errorCallback);
        }

        [Test]
        public void promises_have_sequential_ids()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            Assert.AreEqual(promise1.Id + 1, promise2.Id);
        }


        [Test]
        public void finally_is_called_after_resolve()
        {
            var promise = new Promise<int>();
            int callback = 0;

            promise.Finally(() => ++callback);

            promise.Resolve(0);

            Assert.AreEqual(1, callback);
        }

        [Test]
        public void finally_is_called_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            int callback = 0;

            promise.Finally(() => ++callback);

            promise.Reject(new Exception());

            Assert.AreEqual(1, callback);
        }

        [Test]
        //tc39
        public void resolved_chain_continues_after_finally()
        {
            var promise = new Promise<int>();
            int callback = 0;
            const int expectedValue = 42;

            promise
                .Finally(() => ++callback)
                .Then(x =>
                {
                    Assert.AreEqual(expectedValue, x);
                    ++callback;
                });

            promise.Resolve(expectedValue);

            Assert.AreEqual(2, callback);
        }

        [Test]
        //tc39
        public void rejected_chain_rejects_after_finally()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            int callback = 0;

            promise
                .Finally(() => ++callback)
                .Catch(_ => ++callback);

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void rejected_chain_continues_after_ContinueWith_returning_non_value_promise()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
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
            var promise = new Promise<int>();
            int callback = 0;
            const int expectedValue = 42;
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

            promise.Reject(new Exception());

            Assert.AreEqual(2, callback);
        }

        [Test]
        public void can_chain_promise_generic_after_finally()
        {
            var promise = new Promise<int>();
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

            promise.Resolve(0);

            Assert.AreEqual(2, callback);
        }

        [Test]
        //tc39
        public void can_chain_promise_after_finally()
        {
            var promise = new Promise<int>();
            int callback = 0;

            promise
                .Finally(() => ++callback)
                .Then(_ => ++callback);

            promise.Resolve(0);

            Assert.AreEqual(2, callback);
        }

        [Test]
        //tc39 note: "a throw (or returning a rejected promise) in the finally callback will reject the new promise with that rejection reason."
        public void exception_in_finally_callback_is_caught_by_chained_catch()
        {
            //NOTE: Also tests that the new exception is passed thru promise chain
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
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
            //NOTE: Also tests that the new exception is passed thru promise chain
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
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
            // NOTE: Also tests that the new exception is passed through promise chain
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
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
        public void exception_in_reject_callback_is_caught_by_chained_catch()
        {
            LogAssert.ignoreFailingMessages = true;
            var expectedException = new Exception("Expected");
            Exception actualException = null;

            new Promise<object>((res, rej) => rej(new Exception()))
                .Then(
                    _ => Promise<object>.Resolved(null),
                    _ => throw expectedException
                )
                .Catch(ex => actualException = ex);

            Assert.AreEqual(expectedException, actualException);
        }

        [Test]
        public void rejected_reject_callback_is_caught_by_chained_catch()
        {
            LogAssert.ignoreFailingMessages = true;
            var expectedException = new Exception("Expected");
            Exception actualException = null;

            new Promise<object>((res, rej) => rej(new Exception()))
                .Then(
                    _ => Promise<object>.Resolved(null),
                    _ => Promise<object>.Rejected(expectedException)
                )
                .Catch(ex => actualException = ex);

            Assert.AreEqual(expectedException, actualException);
        }
    }
}
