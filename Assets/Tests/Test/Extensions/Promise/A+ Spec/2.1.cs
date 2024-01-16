using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
using System;
using UnityEngine.TestTools;
namespace Cr7Sund.Framework.PromiseTest.A__Spec
{
    public class _2_1
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Init(new InternalLogger());
        }
        
        // 2.1.1.1.
        [Test]
        public void When_pending_a_promise_may_transition_to_either_the_fulfilled_or_rejected_state()
        {
            LogAssert.ignoreFailingMessages = true;
            var pendingPromise1 = new Promise<object>();
            Assert.AreEqual(PromiseState.Pending, pendingPromise1.CurState);
            pendingPromise1.Resolve(new object());
            Assert.AreEqual(PromiseState.Resolved, pendingPromise1.CurState);

            var pendingPromise2 = new Promise<object>();
            Assert.AreEqual(PromiseState.Pending, pendingPromise2.CurState);
            pendingPromise2.Reject(new Exception());
            Assert.AreEqual(PromiseState.Rejected, pendingPromise2.CurState);
        }

        // 2.1.2
        public class When_fulfilled_a_promise_
        {
            // 2.1.2.1
            [Test]
            public void _must_not_transition_to_any_other_state()
            {
                var fulfilledPromise = new Promise<object>();
                fulfilledPromise.Resolve(new object());

                var promiseException = Assert.Throws<Util.MyException>(() => fulfilledPromise.Resolve(new object()));
                Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
                Assert.AreEqual(PromiseState.Resolved, fulfilledPromise.CurState);
            }

            // 2.1.2.2
            [Test]
            public void _must_have_a_value_which_must_not_change()
            {
                object promisedValue = new object();
                var fulfilledPromise = new Promise<object>();
                int handled = 0;

                fulfilledPromise.Then(v =>
                {
                    Assert.AreEqual(promisedValue, v);
                    ++handled;
                });

                fulfilledPromise.Resolve(promisedValue);

                var promiseException = Assert.Throws<Util.MyException>(() => fulfilledPromise.Resolve(new object()));
                Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
                Assert.AreEqual(1, handled);
            }
        }

        // 2.1.3
        public class When_rejected_a_promise_
        {
            // 2.1.3.1
            [Test]
            public void _must_not_transition_to_any_other_state()
            {
                LogAssert.ignoreFailingMessages = true;
                var rejectedPromise = new Promise<object>();
                rejectedPromise.Reject(new Exception());

                var promiseException = Assert.Throws<Util.MyException>(() => rejectedPromise.Resolve(new object()));
                Assert.AreEqual(PromiseExceptionType.Valid_RESOLVED_STATE, promiseException.Type);
                Assert.AreEqual(PromiseState.Rejected, rejectedPromise.CurState);
            }

            // 2.1.3.21
            [Test]
            public void _must_have_a_reason_which_must_not_change()
            {
                LogAssert.ignoreFailingMessages = true;
                var rejectedPromise = new Promise<object>();
                var reason = new Exception();
                int handled = 0;

                rejectedPromise.Catch(e =>
                {
                    Assert.AreEqual(reason, e);
                    ++handled;
                });

                rejectedPromise.Reject(reason);

                var promiseException = Assert.Throws<Util.MyException>(() => rejectedPromise.Reject(new Exception()));
                Assert.AreEqual(PromiseExceptionType.Valid_REJECTED_STATE, promiseException.Type);
                Assert.AreEqual(1, handled);
            }
        }
    }
}
