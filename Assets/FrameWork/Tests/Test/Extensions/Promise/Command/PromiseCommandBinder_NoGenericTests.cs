using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;

namespace Cr7Sund.Framework.PromiseCommandTest
{

    public class PromiseCommandBinderNoGenericTests
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
            SimplePromise.simulatePromise = new Promise();
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
        public void command_injection()
        {
            injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();

            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.
                Then<TestInjectionCommand>()
                ;


            promiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(1, SimplePromise.result);
        }

    }

}