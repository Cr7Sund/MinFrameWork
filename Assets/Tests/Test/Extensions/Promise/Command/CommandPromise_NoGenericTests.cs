using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using NUnit.Framework;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseCommandTest
{

    public class CommandPromise_NoGenericTests
    {
        [SetUp]
        public void Setup()
        {
            SimplePromise.simulatePromise = new Promise();

            Console.Init(InternalLoggerFactory.Create());
        }

        [TearDown]
        public void Cleanup()
        {
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

            var promise = new CommandPromise();

            promise.Then<SimpleCommandTwo>().Then<SimpleCommandOne>().Then(() => SimplePromise.result -= 200);

            promise.Resolve();
            Assert.AreEqual(((0 + 2) * 3 + 1) * 2 - 200, SimplePromise.result);
        }

        [Test]
        public void command_with_async_operation()
        {
            var promise = new CommandPromise();

            promise.Then<SimpleCommandTwo>().Then<SimpleAsyncCommandOne>().Then<SimpleCommandOne>();

            promise.Resolve();
            Assert.AreEqual(6, SimplePromise.result);

            SimplePromise.simulatePromise.Resolve();
            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_exception_trigger_catch()
        {
            LogAssert.ignoreFailingMessages = true;

            var promise = new CommandPromise();
            var rejectPromise = promise.Then<ExceptionCommand>() as Promise;
            promise.Resolve();
            Assert.NotNull(SimplePromise.exceptionStr);
        }

        [Test]
        public void command_break_chain()
        {
            LogAssert.ignoreFailingMessages = true;

            var promise = new CommandPromise();

            promise.Then<SimpleCommandTwo>().Then<ExceptionCommand>().Then<SimpleCommandOne>();

            promise.Resolve();
            Assert.AreEqual((0 + 2) * 3, SimplePromise.result);
        }


        [Test]
        public void can_handle_onProgress()
        {
            var promise = new CommandPromise();


            promise.Then<SimpleProgressCommand>();

            promise.ReportProgress(1f);

            Assert.AreEqual(1f, SimplePromise.currentProgress);
        }

        [Test]
        public void can_report_simple_progress()
        {
            var promise = new CommandPromise();


            promise.Then<SimpleProgressCommand>();

            for (float progress = 0.25f; progress < 1f; progress += 0.25f)
                promise.ReportProgress(progress);
            promise.ReportProgress(1f);

            Assert.AreEqual(1f, SimplePromise.currentProgress);
        }
    }

}
