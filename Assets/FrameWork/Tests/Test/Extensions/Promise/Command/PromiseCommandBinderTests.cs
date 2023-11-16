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
        private IPromiseCommandBinder<int> promiseBinder;

        [SetUp]
        public void Setup()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            promiseBinder = new PromiseCommandBinder<int>();
            injectionBinder.Injector.Inject(promiseBinder);

            SimplePromise.simulatePromiseOne = new Promise<int>();
            SimplePromise.simulatePromiseSecond = new Promise<int>();
        }

        [TearDown]
        public void Cleanup()
        {
            SimplePromise.result = 0;
        }

        [Test]
        public void command_binder_simple()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.
                Then<SimplePromiseCommandTwo_Generic>()
                .Then<SimplePromiseCommandOne_Generic>()
                .Then<SimplePromiseCommandTwo_Generic>()
                ;


            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual((16 * 3), SimplePromise.result);
        }

        [Test]
        public void command_binder_with_async_operation()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE);

            promiseBinding
            .Then<SimplePromiseCommandTwo_Generic>()
            .Then<SimpleAsyncPromiseCommandOne_Generic>()
            .Then<SimplePromiseCommandOne_Generic>();

            promiseBinder.ReactTo(SomeEnum.ONE, 0);
            Assert.AreEqual(6, SimplePromise.result);

            SimplePromise.simulatePromiseOne.Resolve(0);

            Assert.AreEqual(((((0 + 2) * 3) + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_multiple_async_operation()
        {

            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                 .Then<SimpleAsyncPromiseCommandOne_Generic>()
                 .Then<SimpleAsyncPromiseCommandSecond_Generic>();

            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            SimplePromise.simulatePromiseOne.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(2);

            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_convert_type()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                   .Then<SimplePromiseCommandTwo_Generic>()
                   .ThenConvert<ConvertPromiseCommand, float>();


            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, SimplePromise.floatResult);
        }

        [Test]
        public void command_with_convert_changed_type()
        {
            var promiseBinding = promiseBinder.Bind(SomeEnum.ONE)
                .Then<SimplePromiseCommandTwo_Generic>()
                .ThenConvert<ConvertPromiseCommand, float>()
                .Then<AnotherPromiseCommand, float>();

            promiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 4.2f + 1) * 2, SimplePromise.floatResult);
        }

        [Test]
        public void first_is_resolved_when_first_promise_is_resolved_first()
        {
            promiseBinder.Bind(SomeEnum.ONE)
                .ThenFirst<
                    ExceptionPromiseCommand_Generic,
                    SimplePromiseCommandOne_Generic,
                    SimplePromiseCommandTwo_Generic
                    >()
                 .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(10, SimplePromise.result);
        }


        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first()
        {
            promiseBinder.Bind(SomeEnum.ONE).ThenRace<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }

        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first_chain()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenRace<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
             .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void race_is_resolved_when_first_promise_is_rejected_first()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenRace<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
            .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseOne.Reject(new Exception());
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }


        [Test]
        public void race_is_resolved_when_first_promise_is_resolved_first_in_chain()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenRace<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
            .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);
            SimplePromise.simulatePromiseOne.Reject(new Exception());

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_first_promise_is_resolved_first()
        {
            promiseBinder.Bind(SomeEnum.ONE).ThenAny<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_first_promise_is_resolved_first_chain()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenAny<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
             .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_first_promise_is_rejected_first()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenAny<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
             .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseOne.Reject(new Exception());
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_first_promise_is_resolved_first_in_chain()
        {
            promiseBinder.Bind(SomeEnum.ONE)
            .ThenAny<
                 SimpleAsyncPromiseCommandOne_Generic,
                 SimpleAsyncPromiseCommandSecond_Generic>()
             .Then<SimplePromiseCommandOne_Generic>();


            promiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            SimplePromise.simulatePromiseOne.Reject(new Exception());

            Assert.AreEqual(22, SimplePromise.result);
        }

        // [Test]
        // public void command_no_value_chaining_value()
        // {
        //     promiseBinder.Bind(SomeEnum.ONE)
        //         .Then<SimplePromiseCommandTwo>()
        //         .Then<SimpleAsyncPromiseCommandOne_Generic>()
        //         ;

        //     promiseBinder.ReactTo(SomeEnum.ONE);

        //     Assert.AreEqual(14, SimplePromise.result);
        // }

    }

}