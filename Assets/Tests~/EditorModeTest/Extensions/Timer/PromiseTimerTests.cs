using NUnit.Framework;
using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseTimerTest
{
    public class PromiseTimerTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }
        [Test]
        public void wait_until_elapsedUpdates_resolves_when_predicate_is_true()
        {
            var testObject = new PromiseTimer();

            const int testFrame = 3;
            bool hasResolved = false;

            testObject.WaitUntil(timeData => timeData.elapsedUpdates == testFrame)
                .Then(() => hasResolved = true)
                .Done();

            Assert.False(hasResolved);

            testObject.Update(1);
            testObject.Update(2);
            testObject.Update(3);

            Assert.True(hasResolved);
        }

        [Test]
        public void wait_for_doesnt_resolve_before_specified_time()
        {
            var testObject = new PromiseTimer();

            const int testTime = 2000;
            bool hasResolved = false;

            testObject.WaitFor(testTime)
                .Then(() => hasResolved = true)
                .Done();

            testObject.Update(1000);

            Assert.AreEqual(false, hasResolved);
        }

        [Test]
        public void wait_for_resolves_after_specified_time()
        {
            var testObject = new PromiseTimer();

            const int testTime = 1000;
            bool hasResolved = false;

            testObject.WaitFor(testTime)
                .Then(() => hasResolved = true)
                .Done();

            testObject.Update(2000);

            Assert.AreEqual(true, hasResolved);
        }

        [Test]
        public void wait_until_resolves_when_predicate_is_true()
        {
            var testObject = new PromiseTimer();

            bool hasResolved = false;

            bool doResolve = false;

            testObject.WaitUntil(timeData => doResolve)
                .Then(() => hasResolved = true)
                .Done();

            Assert.AreEqual(false, hasResolved);

            doResolve = true;
            testObject.Update(1000);

            Assert.AreEqual(true, hasResolved);
        }

        [Test]
        public void wait_while_resolves_when_predicate_is_false()
        {
            var testObject = new PromiseTimer();

            bool hasResovled = false;

            bool doWait = true;

            testObject.WaitWhile(timeData => doWait)
                .Then(() => hasResovled = true)
                .Done();

            Assert.AreEqual(false, hasResovled);

            doWait = false;
            testObject.Update(1000);

            Assert.AreEqual(true, hasResovled);
        }

        [Test]
        public void predicate_is_removed_from_timer_after_exception_is_thrown()
        {
            AssertHelper.IgnoreFailingMessages();
            var testObject = new PromiseTimer();

            int runCount = 0;

            testObject
                .WaitUntil(timeData =>
                {
                    runCount++;

                    throw new NotImplementedException();
                })
                .Done();

            testObject.Update(1000);
            testObject.Update(1000);

            Assert.AreEqual(1, runCount);
        }

        [Test]
        public void when_promise_is_not_cancelled_by_user_resolve_promise()
        {
            var testObject = new PromiseTimer();
            bool hasResolved = false;
            Exception caughtException = null;


            var promise = testObject
                .WaitUntil(timeData => timeData.elapsedTime > 1000)
                .Then(() => hasResolved = true)
                .Catch(ex => caughtException = ex);

            promise.Done(null, ex => caughtException = ex);

            testObject.Update(1000);

            Assert.AreEqual(hasResolved, false);

            testObject.Update(1000);

            Assert.AreEqual(caughtException, null);
            Assert.AreEqual(hasResolved, true);
        }

        [Test]
        public void when_promise_is_cancelled_by_user_reject_promise()
        {
            AssertHelper.IgnoreFailingMessages();
            var testObject = new PromiseTimer();
            Exception caughtException = null;


            var promise = testObject
                .WaitUntil(timeData => timeData.elapsedTime > 1000);
            promise.Catch(ex => caughtException = ex);

            promise.Done(null, ex => caughtException = ex);

            testObject.Update(1000);
            testObject.Cancel(promise);
            testObject.Update(1000);

            Assert.AreEqual(typeof(PromiseTimerException), caughtException.GetType());
            Assert.AreEqual(PromiseTimerExceptionType.CANCEL_TIMER, ((PromiseTimerException)caughtException).Type);
            Assert.AreEqual(caughtException.Message, "Promise was cancelled by user.");
        }

        [Test]
        public void when_predicate_throws_exception_reject_promise()
        {
            AssertHelper.IgnoreFailingMessages();
            var testObject = new PromiseTimer();

            var expectedException = new Exception();
            Exception caughtException = null;


            testObject
                .WaitUntil(timeData => throw expectedException)
                .Catch(ex => caughtException = ex)
                .Done();

            testObject.Update(1000);

            Assert.AreEqual(expectedException, caughtException);
        }

        [Test]
        public void all_promises_are_updated_when_a_pending_promise_is_resolved_during_update()
        {
            var testObject = new PromiseTimer();

            int p1Updates = 0;
            int p2Updates = 0;
            int p3Updates = 0;

            testObject
                .WaitUntil(timeData =>
                {
                    p1Updates++;

                    return false;
                });

            testObject
                .WaitUntil(timeData =>
                {
                    p2Updates++;

                    return true;
                });

            testObject
                .WaitUntil(timeData =>
                {
                    p3Updates++;

                    return false;
                });

            testObject.Update(10);

            Assert.AreEqual(1, p1Updates);
            Assert.AreEqual(1, p2Updates);
            Assert.AreEqual(1, p3Updates);
        }

        [Test]
        public void all_promises_are_updated_when_a_pending_promise_is_canceled_during_update()
        {
            AssertHelper.IgnoreFailingMessages();
            var testObject = new PromiseTimer();

            int p1Updates = 0;
            int p2Updates = 0;
            int p3Updates = 0;

            var p1 = testObject
                .WaitUntil(timeData =>
                {
                    p1Updates++;

                    return false;
                });

            testObject
                .WaitUntil(timeData =>
                {
                    p2Updates++;

                    return true;
                })
                .Then(() =>
                {
                    testObject.Cancel(p1);
                });

            testObject
                .WaitUntil(timeData =>
                {
                    p3Updates++;

                    return false;
                });

            testObject.Update(10);

            Assert.AreEqual(1, p1Updates);
            Assert.AreEqual(1, p2Updates);
            Assert.AreEqual(1, p3Updates);
        }
    }
}
