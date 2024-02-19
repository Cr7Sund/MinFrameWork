using Cr7Sund.PackageTest.IOC;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseCommandTest
{

    public class CommandPromiseTest
    {
        [SetUp]
        public void SetUp()
        {
            SimplePromise.simulatePromiseOne = new Promise<int>();
            SimplePromise.simulatePromiseSecond = new Promise<int>();
            SimplePromise.simulatePromise = new Promise();

            Debug.Init(new InternalLogger());
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
            var donePromise = promise.Then<SimpleCommandTwoGeneric>().Then<SimpleCommandOneGeneric>().Then(value => value - 200);
            promise.Resolve(0);
            Assert.AreEqual(((0 + 2) * 3 + 1) * 2 - 200, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_all_resolve_as_Once()
        {
            var promise = new CommandPromise<int>();
            promise.IsOnceOff = true;
            var donePromise = promise.Then<SimpleCommandTwoGeneric>().Then<SimpleCommandOneGeneric>().Then(value => value - 200);
            promise.Resolve(0);
            Assert.AreEqual(((0 + 2) * 3 + 1) * 2 - 200, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_async_operation()
        {
            var promise = new CommandPromise<int>();
            var donePromise1 = promise.Then<SimpleCommandTwoGeneric>();
            var donePromise2 = donePromise1.Then<SimpleAsyncCommandOneGeneric>().Then<SimpleCommandOneGeneric>();

            promise.Resolve(0);
            Assert.AreEqual(6, ((Promise<int>)donePromise1).Test_GetResolveValue());

            SimplePromise.simulatePromiseOne.Resolve(0);
            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, ((Promise<int>)donePromise2).Test_GetResolveValue());
        }

        [Test]
        public void command_with_async_operation_asOnce()
        {
            var promise = new CommandPromise<int>();
            promise.IsOnceOff = true;

            var donePromise1 = promise.Then<SimpleCommandTwoGeneric>();
            var donePromise2 = donePromise1.Then<SimpleAsyncCommandOneGeneric>().Then<SimpleCommandOneGeneric>();

            promise.Resolve(0);
            Assert.AreEqual(6, ((Promise<int>)donePromise1).Test_GetResolveValue());

            SimplePromise.simulatePromiseOne.Resolve(0);
            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, ((Promise<int>)donePromise2).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleAsyncCommandSecondGeneric>();

            promise.Resolve(0);

            SimplePromise.simulatePromiseOne.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(2);

            Assert.AreEqual(((0 + 1 + 3) * 5 + 2 + 1) * 2, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation_misc_order_still_sequence_order()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleAsyncCommandSecondGeneric>();

            promise.Resolve(0);

            SimplePromise.simulatePromiseSecond.Resolve(2);
            SimplePromise.simulatePromiseOne.Resolve(1);

            Assert.AreEqual(((0 + 1 + 3) * 5 + 2 + 1) * 2, ((Promise<int>)donePromise).Test_GetResolveValue());
            Assert.AreNotEqual(((0 + 2 + 1) * 2 + 1 + 3) * 5, ((Promise<int>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_multiple_async_operation_until_chaining_resolve()
        {
            var promise = new CommandPromise<int>();
            promise
                .Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleAsyncCommandSecondGeneric>();

            promise.Resolve(0);

            SimplePromise.simulatePromiseSecond.Resolve(2);
            Assert.AreEqual(0, SimplePromise.result);

            SimplePromise.simulatePromiseOne.Resolve(1);
            Assert.AreEqual(((0 + 1 + 3) * 5 + 2 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_convert_type()
        {
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleCommandTwoGeneric>()
                .Then<ConvertCommand, float>();

            promise.Resolve(0);

            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, ((CommandPromise<float>)donePromise).Test_GetResolveValue());
        }

        [Test]
        public void command_with_convert_changed_type()
        {
            var promise = new CommandPromise<int>();
            var rejectPromise = promise
                .Then<SimpleCommandTwoGeneric>()
                .Then<ConvertCommand, float>()
                .Then<AnotherCommand>();

            promise.Resolve(0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 4.2f + 1) * 2, ((CommandPromise<float>)rejectPromise).Test_GetResolveValue());
        }


        [Test]
        public void command_exception_rejectedState()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new CommandPromise<int>();
            var rejectPromise = promise.Then<ExceptionCommandGeneric>() as Promise<int>;

            promise.Resolve(0);

            Assert.AreEqual(PromiseState.Resolved, promise.CurState);
            Assert.AreEqual(PromiseState.Rejected, rejectPromise.CurState);
        }

        [Test]
        public void command_exception_trigger_catch()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new CommandPromise<int>();
            var rejectPromise = promise.Then<ExceptionCommandGeneric>() as Promise;
            promise.Resolve(0);

            Assert.NotNull(SimplePromise.exceptionStr);
        }

        [Test]
        public void command_break_chain()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new CommandPromise<int>();
            var donePromise = promise.Then<SimpleCommandTwoGeneric>();
            ICommandPromise<int> finalPromise = donePromise.Then<ExceptionCommandGeneric>().Then<SimpleCommandOneGeneric>();
            promise.Resolve(0);


            Assert.AreEqual((0 + 2) * 3, ((Promise<int>)donePromise).Test_GetResolveValue());
            Assert.AreEqual(0, ((Promise<int>)finalPromise).Test_GetResolveValue());
        }

        [Test]
        public void handle_rejected_catch_but_break_chain()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new CommandPromise<int>();
            var finalPromise = promise.Then<SimpleCommandTwoGeneric>()
                .Then<SimpleAsyncException_Generic>()
                .Then<SimpleCommandTwoGeneric>();

            promise.Resolve(1);

            Assert.AreEqual(9, SimplePromise.result);
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

            for (float progress = 0.25f; progress < 1f; progress += 0.25f)
                promise.ReportProgress(progress);
            promise.ReportProgress(1f);

            Assert.AreEqual(1f, SimplePromise.currentProgress);
        }

        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first_chain_promise()
        {
            var commands = new Command<int>[]
            {
                new SimpleAsyncCommandOneGeneric(), new SimpleAsyncCommandSecondGeneric()
            };

            var promises = new[]
            {
                new CommandPromise<int>(), new CommandPromise<int>()
            };

            var promise = new CommandPromise<int>();
            promise
                .ThenRace(promises, commands)
                .Then<SimpleCommandOneGeneric>()
                ;

            promise.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void command_all_simple_command()
        {
            var commands = new Command<int>[]
            {
                new SimpleCommandTwoGeneric(), new SimpleCommandTwoGeneric(),
                new SimpleCommandOneGeneric()
            };

            var promises = new[]
            {
                new CommandPromise<int>(), new CommandPromise<int>(),
                new CommandPromise<int>()
            };

            var promise = new CommandPromise<int>();
            promise
                .ThenAll(promises, commands)
                ;

            promise.Resolve(1);

            Assert.AreEqual(4, SimplePromise.result);
        }

        [Test]
        public void command_all_simple_command_chain_all_result()
        {
            var commands = new Command<int>[]
            {
                new SimpleCommandTwoGeneric(), new SimpleCommandTwoGeneric(),
                new SimpleCommandOneGeneric()
            };

            var promises = new[]
            {
                new CommandPromise<int>(), new CommandPromise<int>(),
                new CommandPromise<int>()
            };

            var promise = new CommandPromise<int>();
            promise
                .ThenAll(promises, commands)
                .Then<ConvertEnumerableCommand, int>();

            promise.Resolve(1);

            Assert.AreEqual(200, SimplePromise.result);
        }
    }

}
