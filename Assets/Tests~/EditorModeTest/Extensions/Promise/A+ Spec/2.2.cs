using NUnit.Framework;
using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseTest.A__Spec
{
    public class _2_2
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }
        
        // 2.2.1
        public class _both_onFulfilled_and_onRejected_are_optional_arguments_
        {
            [SetUp]
            public void SetUp()
            {
                Console.Init(InternalLoggerFactory.Create());
            }
            
            // 2.2.1.1
            [Test]
            public void _if_onFulfilled_is_not_a_function_it_must_be_ignored()
            {
                var promise = new Promise<object>();

                var resultPromise = promise
                    .Then(
                        null,
                        ex => { }
                    );

                int resolves = 0;
                int errors = 0;
                resultPromise.Then(() => ++resolves);
                resultPromise.Catch(ex => ++errors);

                promise.Resolve(new object());

                Assert.AreEqual(1, resolves);
                Assert.AreEqual(0, errors);
            }

            // 2.2.1.2
            [Test]
            public void _if_onRejected_is_not_a_function_it_must_be_ignored_1()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise = new Promise<object>();

                var resultPromise = promise
                    .Then(
                        v => Promise.Resolved(),
                        null
                    );

                int resolved = 0;
                int errors = 0;
                var e = new Exception();
                resultPromise.Then(() => ++resolved);
                resultPromise.Catch(ex =>
                {
                    Assert.AreEqual(e, ex);
                    ++errors;
                });

                promise.RejectWithoutDebug(e);

                Assert.AreEqual(0, resolved);
                Assert.AreEqual(1, errors);
            }

            [Test]
            public void _if_onRejected_is_not_a_function_it_must_be_ignored_2()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise = new Promise<object>();

                var resultPromise = promise
                    .Then(
                        v => Promise<object>.Resolved(new object()),
                        null
                    );

                int resolved = 0;
                int errors = 0;
                var e = new Exception();
                resultPromise.Then(v => ++resolved);
                resultPromise.Catch(ex =>
                {
                    Assert.AreEqual(e, ex);
                    ++errors;
                });

                promise.RejectWithoutDebug(e);

                Assert.AreEqual(0, resolved);
                Assert.AreEqual(1, errors);
            }
        }

        // 2.2.2
        public class _if_onFulfilled_is_a_function_
        {
            // 2.2.2.1
            [Test]
            public void _it_must_be_called_after_promise_is_fulfilled_with_promises_value_as_its_first_argument()
            {
                var promise = new Promise<object>();

                object promisedValue = new object();
                bool resolved = false;

                promise.Then(
                    v =>
                    {
                        Assert.AreEqual(promisedValue, v);
                        resolved = true;
                    },
                    null
                );

                promise.Resolve(promisedValue);

                Assert.True(resolved);
            }

            // 2.2.2.2
            [Test]
            public void _it_must_not_be_called_before_promise_is_fulfilled()
            {
                var promise = new Promise<object>();
                bool resolved = false;

                promise.Then(
                    v => resolved = true,
                    null
                );

                Assert.False(resolved);
            }

            // 2.2.2.3
            [Test]
            public void _it_must_not_be_called_more_than_once()
            {
                var promise = new Promise<object>();
                object promisedValue = new object();
                int resolved = 0;

                promise.Then(
                    v => ++resolved,
                    null
                );

                promise.Resolve(promisedValue);

                Assert.AreEqual(1, resolved);
            }
        }

        // 2.2.3
        public class _If_onRejected_is_a_function_
        {
            [SetUp]
            public void SetUp()
            {
                Console.Init(InternalLoggerFactory.Create());
            }
            
            // 2.2.3.1
            [Test]
            public void _it_must_be_called_after_promise_is_rejected_with_promises_reason_as_its_first_argument()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise = new Promise<object>();
                var rejectedReason = new Exception();
                bool errored = false;

                promise.Then(
                    v => { },
                    ex =>
                    {
                        Assert.AreEqual(rejectedReason, ex);
                        errored = true;
                    }
                );

                promise.RejectWithoutDebug(rejectedReason);

                Assert.True(errored);
            }

            // 2.2.3.2
            [Test]
            public void _it_must_not_be_called_before_promise_is_rejected()
            {
                var promise = new Promise<object>();
                bool errored = false;

                promise.Then(
                    v => { },
                    ex => errored = true
                );

                Assert.False(errored);
            }

            // 2.2.3.3
            [Test]
            public void _it_must_not_be_called_more_than_once()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise = new Promise<object>();
                var rejectedReason = new Exception();
                int errored = 0;

                promise.Then(
                    v => { },
                    ex => ++errored
                );

                promise.RejectWithoutDebug(rejectedReason);

                Assert.AreEqual(1, errored);
            }
        }

        // 2.2.4
        // Not really appropriate in C#.

        // 2.2.5
        // Not really appropriate in C#.

        // 2.2.6
        public class then_may_be_called_multiple_times_on_the_same_promise_
        {
            [SetUp]
            public void SetUp()
            {
                Console.Init(InternalLoggerFactory.Create());
            }
            
            // 2.2.6.1
            [Test]
            public void _when_promise_is_fulfilled_all_respective_onFulfilled_callbacks_must_execute_in_the_order_of_their_originating_calls_to_then_1()
            {
                var promise = new Promise<object>();

                int order = 0;

                promise.Then(_ =>
                {
                    Assert.AreEqual(1, ++order);
                });
                promise.Then(_ =>
                {
                    Assert.AreEqual(2, ++order);
                });
                promise.Then(_ =>
                {
                    Assert.AreEqual(3, ++order);
                });

                promise.Resolve(new object());

                Assert.AreEqual(3, order);
            }

            [Test]
            public void _when_promise_is_fulfilled_all_respective_onFulfilled_callbacks_must_execute_in_the_order_of_their_originating_calls_to_then_2()
            {
                var promise = new Promise<object>();

                int order = 0;

                promise.Then(_ =>
                {
                    Assert.AreEqual(1, ++order);

                    return Promise<object>.Resolved(new object());
                });

                promise.Then(_ =>
                {
                    Assert.AreEqual(2, ++order);

                    return Promise<object>.Resolved(new object());
                });

                promise.Then(_ =>
                {
                    Assert.AreEqual(3, ++order);

                    return Promise<object>.Resolved(new object());
                });

                promise.Resolve(new object());

                Assert.AreEqual(3, order);
            }

            // 2.2.6.2
            [Test]
            public void _when_promise_is_rejected_all_respective_onRejected_callbacks_must_execute_in_the_order_of_their_originating_calls_to_then()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise = new Promise<object>();

                int order = 0;

                promise.Catch(_ =>
                {
                    Assert.AreEqual(1, ++order);
                });
                promise.Catch(_ =>
                {
                    Assert.AreEqual(2, ++order);
                });
                promise.Catch(_ =>
                {
                    Assert.AreEqual(3, ++order);
                });

                promise.RejectWithoutDebug(new Exception());

                Assert.AreEqual(3, order);
            }
        }

        // 2.2.7
        public class then_must_return_a_promise
        {
            [SetUp]
            public void SetUp()
            {
                Console.Init(InternalLoggerFactory.Create());
            }
            // 2.2.7.3
            [Test]
            public void _If_onFulfilled_is_not_a_function_and_promise1_is_fulfilled_promise2_must_be_fulfilled_with_the_same_value_as_promise1()
            {
                var promise1 = new Promise<object>();

                var promise2 = promise1.Catch(_ =>
                    throw new Exception("There shouldn't be an error")
                );

                object promisedValue = new object();
                int promise2ThenHandler = 0;

                promise2.Then(v =>
                {
                    Assert.AreEqual(promisedValue, v);
                    ++promise2ThenHandler;
                });

                promise1.Resolve(promisedValue);

                Assert.AreEqual(1, promise2ThenHandler);
            }

            [Test]
            public void _If_onRejected_is_not_a_function_and_promise1_is_rejected_promise2_must_be_rejected_with_the_same_reason_as_promise1()
            {
                AssertHelper.IgnoreFailingMessages();
                var promise1 = new Promise<object>();

                var promise2 = promise1.Then(_ =>
                    throw new Exception("There shouldn't be a then callback")
                );

                var e = new Exception();
                int promise2CatchHandler = 0;

                promise2.Catch(ex =>
                {
                    Assert.AreEqual(e, ex);
                    ++promise2CatchHandler;
                });

                promise1.RejectWithoutDebug(e);

                Assert.AreEqual(1, promise2CatchHandler);
            }

            // 2.2.7.1
            public class _If_either_onFulfilled_or_onRejected_returns_a_value_x_fulfill_promise_with_x
            {
                [Test]
                public void _when_promise1_is_resolved()
                {
                    var promise1 = new Promise<object>();

                    object promisedValue1 = new object();
                    object promisedValue2 = new object();

                    var promise2 =
                        promise1.Then(_ => promisedValue2);

                    int promise1ThenHandler = 0;
                    promise1.Then(v =>
                    {
                        Assert.AreEqual(promisedValue1, v);
                        ++promise1ThenHandler;

                    });

                    int promise2ThenHandler = 0;
                    promise2.Then(v =>
                    {
                        Assert.AreEqual(promisedValue2, v);
                        ++promise2ThenHandler;

                    });

                    promise1.Resolve(promisedValue1);

                    Assert.AreEqual(1, promise1ThenHandler);
                    Assert.AreEqual(1, promise2ThenHandler);
                }

                [Test]
                public void _when_promise1_is_rejected_with_no_value_in_catch()
                {
                    AssertHelper.IgnoreFailingMessages();
                    bool callbackInvoked = false;

                    new Promise<object>((res, rej) => rej(new Exception()))
                        .Catch(_ => { })
                        .Then(() => callbackInvoked = true);

                    Assert.True(callbackInvoked);
                }

                [Test]
                public void _when_promise1_is_rejected_with_no_value_in_then()
                {
                    AssertHelper.IgnoreFailingMessages();
                    bool callbackInvoked = false;
                    bool resolveHandlerInvoked = false;
                    bool rejectHandlerInvoked = false;

                    new Promise((res, rej) => rej(new Exception()))
                        .Then(
                            () => { resolveHandlerInvoked = true; },
                            ex => { rejectHandlerInvoked = true; }
                        )
                        .Then(() => callbackInvoked = true);

                    Assert.True(callbackInvoked);
                    Assert.False(resolveHandlerInvoked);
                    Assert.True(rejectHandlerInvoked);
                }

                [Test]
                public void _when_promise1_is_rejected_with_value_in_catch()
                {
                    AssertHelper.IgnoreFailingMessages();
                    string expectedValue = "Value returned from Catch";
                    string actualValue = string.Empty;

                    new Promise<string>((res, rej) => rej(new Exception()))
                        .Catch(_ => expectedValue)
                        .Then(val => actualValue = val);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                [Test]
                public void _when_promise1_is_rejected_with_value_in_then()
                {
                    AssertHelper.IgnoreFailingMessages();
                    string expectedValue = "Value returned from reject handler";
                    string actualValue = string.Empty;

                    new Promise<string>((res, rej) => rej(new Exception()))
                        .Then(
                            _ => Promise<string>.Resolved(string.Empty),
                            _ => Promise<string>.Resolved(expectedValue)
                        )
                        .Then(val => actualValue = val);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                [Test]
                public void _when_non_generic_promise1_is_rejected()
                {
                    AssertHelper.IgnoreFailingMessages();
                    bool callbackInvoked = false;

                    new Promise((res, rej) => rej(new Exception()))
                        .Catch(_ => { })
                        .Then(() => callbackInvoked = true);

                    Assert.True(callbackInvoked);
                }
            }

            // 2.2.7.2
            public class _if_either_onFulfilled_or_onRejected_throws_an_exception_e_promise2_must_be_rejected_with_e_as_the_reason
            {
                [SetUp]
                public void SetUp()
                {
                    Console.Init(InternalLoggerFactory.Create());
                }
                
                [Test]
                public void _when_promise1_is_resolved_1()
                {
                    AssertHelper.IgnoreFailingMessages();
                    var promise1 = new Promise<object>();

                    var e = new Exception();
                    Func<object, IPromise<object>> thenHandler = _ => throw e;

                    var promise2 =
                        promise1.Then(thenHandler);

                    promise1.Catch(_ => throw new Exception("This shouldn't happen!"));

                    int errorHandledForPromise2 = 0;
                    promise2.Catch(ex =>
                    {
                        Assert.AreEqual(e, ex);

                        ++errorHandledForPromise2;
                    });

                    promise1.Resolve(new object());

                    Assert.AreEqual(1, errorHandledForPromise2);
                }

                [Test]
                public void _when_promise1_is_resolved_2()
                {
                    AssertHelper.IgnoreFailingMessages();
                    var promise1 = new Promise<object>();

                    var e = new Exception();
                    Action<object> thenHandler = _ => throw e;

                    var promise2 =
                        promise1.Then(thenHandler);

                    promise1.Catch(_ => throw new Exception("This shouldn't happen!"));

                    int errorHandledForPromise2 = 0;
                    promise2.Catch(ex =>
                    {
                        Assert.AreEqual(e, ex);

                        ++errorHandledForPromise2;
                    });

                    promise1.Resolve(new object());

                    Assert.AreEqual(1, errorHandledForPromise2);
                }

                [Test]
                public void _when_promise1_is_rejected()
                {
                    AssertHelper.IgnoreFailingMessages();
                    var promise1 = new Promise<object>();

                    var e = new Exception();
                    var promise2 =
                        promise1.Catch(_ => throw e);

                    promise1.Catch(_ => throw new Exception("This shouldn't happen!"));

                    int errorHandledForPromise2 = 0;
                    promise2.Catch(ex =>
                    {
                        Assert.AreEqual(e, ex);

                        ++errorHandledForPromise2;
                    });

                    promise1.RejectWithoutDebug(new Exception());

                    Assert.AreEqual(1, errorHandledForPromise2);
                }
            }
        }
    }
}
