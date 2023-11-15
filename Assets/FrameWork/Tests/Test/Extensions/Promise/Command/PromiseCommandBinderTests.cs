using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;
using UnityEditor.VersionControl;

namespace Cr7Sund.Framework.PromiseCommandTest
{

    public class PromiseCommandBinderTests
    {
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;
        private IPromiseCommandBinder promiseBinder;

        [SetUp]
        public void Setup()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            promiseBinder = new PromiseCommandBinder();
            injectionBinder.Injector.Inject(promiseBinder);

            SimplePromise.simulatePromiseOne = new Promise<int>();
            SimplePromise.simulatePromiseSecond = new Promise<int>();
        }

        [TearDown]
        public void Cleanup()
        {
            SimplePromise.result = 0;
            SimplePromise.simulatePromise = null;
        }

        [Test]
        public void command_binder_simple()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.
                Then<SimplePromiseCommandTwo>().Then<SimplePromiseCommandOne>().Then<SimplePromiseCommandTwo>()
                ;


            promiseBinder.ReactTo(SomeEnum.ONE);

            Assert.AreEqual((16 * 3), SimplePromise.result);
        }

        [Test]
        public void command_binder_with_async_operation()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE);

            promiseBinding.Then<SimplePromiseCommandTwo>().Then<SimpleAsyncPromiseCommandOne>().Then<SimplePromiseCommandOne>();

            promiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(6, SimplePromise.result);

            SimplePromise.simulatePromise.Resolve();

            Assert.AreEqual(((((0 + 2) * 3) + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_multiple_async_operation()
        {

            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                 .Then<SimpleAsyncPromiseCommandOne_Generic, int>()
                 .Then<SimpleAsyncPromiseCommandSecond_Generic, int>();

            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            SimplePromise.simulatePromiseOne.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(2);

            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_convert_type()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                   .Then<SimplePromiseCommandTwo_Generic, int>()
                   .Then<ConvertPromiseCommand, int, float>();


            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, SimplePromise.floatResult);
        }

        [Test]
        public void command_with_convert_changed_type()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                .Then<SimplePromiseCommandTwo_Generic, int>()
                .Then<ConvertPromiseCommand, int, float>()
                .Then<AnotherPromiseCommand, float>();

            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 4.2f + 1) * 2, SimplePromise.floatResult);
        }

        [Test]
        public void command_all_simple_command()
        {
            promiseBinder.Bind(SomeEnum.ONE).ThenAll(
                new SimplePromiseCommandTwo_Generic(),
                new SimplePromiseCommandOne_Generic(),
                new SimplePromiseCommandTwo_Generic()
                );

            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(27, SimplePromise.result);
        }

        [Test]
        public void command_all_simple_command_generic()
        {
            promiseBinder.Bind(SomeEnum.ONE).ThenAll<
                 SimplePromiseCommandTwo_Generic,
                 SimplePromiseCommandOne_Generic,
                 SimplePromiseCommandTwo_Generic, int>();


            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(27, SimplePromise.result);
        }
    }

}