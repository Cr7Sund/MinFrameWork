using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;
using UnityEditor.VersionControl;

namespace Cr7Sund.Framework.PromiseCommandTest
{

    public class PromiseCommandTests
    {
        [SetUp]
        public void SetUp()
        {
            SimplePromise.simulatePromiseOne = new Promise<int>();
            SimplePromise.simulatePromiseSecond = new Promise<int>();
        }

        [TearDown]
        public void Cleanup()
        {
            SimplePromise.simulatePromiseOne = null;
            SimplePromise.simulatePromiseSecond = null;
            SimplePromise.result = 0;
            SimplePromise.simulatePromise = null;
            SimplePromise.exceptionStr = null;
        }


        [Test]
        public void command_all_resolve()
        {
            // equal to
            // var promise1 = new Promise();
            // promise1.Then(new PromiseCommand_1().Resolve).Then(new PromiseCommand_1().Resolve);

            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimplePromiseCommandTwo_Generic>().Then<SimplePromiseCommandOne_Generic>().Then((value) => value - 200);
            promise.Resolve(0);
            Assert.AreEqual((((0 + 2) * 3) + 1) * 2 - 200, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_async_operation()
        {
            var promise = new CommandPromise<int>();
            var donePromise1 = promise.Then<SimplePromiseCommandTwo_Generic>();
            var donePromise2 = donePromise1.Then<SimpleAsyncPromiseCommandOne_Generic>().Then<SimplePromiseCommandOne_Generic>();

            promise.Resolve(0);
            Assert.AreEqual(6, ((Promise<int>)donePromise1).Test_GetResolveValue());

            SimplePromise.simulatePromiseOne.Resolve(0);
            Assert.AreEqual(((((0 + 2) * 3) + 3) * 5 + 1) * 2, ((Promise<int>)donePromise2).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleAsyncPromiseCommandOne_Generic>()
            .Then<SimpleAsyncPromiseCommandSecond_Generic>();

            promise.Resolve(0);

            SimplePromise.simulatePromiseOne.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(2);

            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation_misc_order_still_sequence_order()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleAsyncPromiseCommandOne_Generic>()
            .Then<SimpleAsyncPromiseCommandSecond_Generic>();

            promise.Resolve(0);

            SimplePromise.simulatePromiseSecond.Resolve(2);
            SimplePromise.simulatePromiseOne.Resolve(1);

            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, ((Promise<int>)donePromise).Test_GetResolveValue());
            Assert.AreNotEqual((((0 + 2 + 1) * 2) + 1 + 3) * 5, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation_until_chaining_resolve()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleAsyncPromiseCommandOne_Generic>()
            .Then<SimpleAsyncPromiseCommandSecond_Generic>();
            promise.Resolve(0);

            SimplePromise.simulatePromiseSecond.Resolve(2);
            Assert.AreEqual(1, SimplePromise.result);

            SimplePromise.simulatePromiseOne.Resolve(1);
            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_convert_type()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimplePromiseCommandTwo_Generic>()
                .Then<ConvertPromiseCommand, float>();

            promise.Resolve(0);

            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, ((CommandPromise<float>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_convert_changed_type()
        {
            var promise = new CommandPromise<int>();
            var rejectPromise = promise.Then<SimplePromiseCommandTwo_Generic>()
                .Then<ConvertPromiseCommand, float>()
                .Then<AnotherPromiseCommand>();

            promise.Resolve(0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 4.2f + 1) * 2, ((CommandPromise<float>)rejectPromise).Test_GetResolveValue());
        }


        [Test]
        public void command_exception_rejectedState()
        {
            var promise = new CommandPromise<int>();
            var rejectPromise = promise.Then<ExceptionPromiseCommand_Generic>() as Promise<int>;
            promise.Resolve(0);

            Assert.AreEqual(PromiseState.Resolved, promise.CurState);
            Assert.AreEqual(PromiseState.Rejected, rejectPromise.CurState);
        }

        [Test]
        public void command_exception_trigger_catch()
        {
            var promise = new CommandPromise<int>();
            var rejectPromise = promise.Then<ExceptionPromiseCommand_Generic>() as Promise;
            promise.Resolve(0);
            Assert.NotNull(SimplePromise.exceptionStr);
        }

        [Test]
        public void command_break_chain()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimplePromiseCommandTwo_Generic>();
            var finalPromise = donePromise.Then<ExceptionPromiseCommand_Generic>().Then<SimplePromiseCommandOne_Generic>();

            promise.Resolve(0);
            Assert.AreEqual((0 + 2) * 3, ((Promise<int>)donePromise).Test_GetResolveValue());
            Assert.Null(((Promise<int>)finalPromise).Test_GetResolveValue());
        }

        [Test]
        public void handle_rejected_catch_but_break_chain()
        {
            var promise = new CommandPromise<int>();
            var finalPromise = promise.Then<SimplePromiseCommandTwo_Generic>()
                 .Then<SimpleAsyncException_Generic>()
                 .Then<SimplePromiseCommandTwo_Generic>();

            promise.Resolve(0);
            Assert.AreEqual(-1, SimplePromise.result);
        }


        [Test]
        public void can_handle_onProgress()
        {
            var promise = new CommandPromise<int>();

            promise.Then<SimpleProgressCommand_Generic>();

            promise.ReportProgress(1f);

            Assert.AreEqual(1f, SimplePromise.currentProgress);
        }

        [Test]
        public void can_report_simple_progress()
        {
            var promise = new CommandPromise<int>();

            promise.Then<SimpleProgressCommand_Generic>();

            for (var progress = 0.25f; progress < 1f; progress += 0.25f)
                promise.ReportProgress(progress);
            promise.ReportProgress(1f);

            Assert.AreEqual(1f, SimplePromise.currentProgress);
        }



    }

}