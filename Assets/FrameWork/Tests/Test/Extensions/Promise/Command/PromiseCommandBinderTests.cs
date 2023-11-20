using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;

namespace Cr7Sund.Framework.PromiseCommandTest
{
    public class PromiseCommandBinderTests
    {
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;
        private ICommandPromiseBinder<int> _commandPromiseBinder;
        private ICommandBinder commandBinder;

        [SetUp]
        public void Setup()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();
            commandBinder = new CommandBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);
            injectionBinder.Bind<ICommandBinder>().To(commandBinder);

            _commandPromiseBinder = new CommandPromiseBinder<int>();
            injectionBinder.Injector.Inject(_commandPromiseBinder);

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
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.Then<SimpleCommandTwoGeneric>()
                    .Then<SimpleCommandOneGeneric>()
                    .Then<SimpleCommandTwoGeneric>()
                ;


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual((16 * 3), SimplePromise.result);
        }

        [Test]
        public void command_binder_with_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            promiseBinding
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleCommandOneGeneric>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);
            Assert.AreEqual(6, SimplePromise.result);

            SimplePromise.simulatePromiseOne.Resolve(0);

            Assert.AreEqual(((((0 + 2) * 3) + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_multiple_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleAsyncCommandSecondGeneric>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            SimplePromise.simulatePromiseOne.Resolve(1);
            SimplePromise.simulatePromiseSecond.Resolve(2);

            Assert.AreEqual((((0 + 1 + 3) * 5) + 2 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void command_with_convert_type()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandTwoGeneric>()
                .ThenConvert<ConvertCommand, float>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, SimplePromise.floatResult);
        }

        [Test]
        public void command_with_convert_continue_with_changed_type()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandTwoGeneric>()
                .ThenConvert<ConvertCommand, float>()
                .Then<AnotherCommand, float>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 4.2f + 1) * 2, SimplePromise.floatResult);
        }

        [Test]
        public void then_first_return_first_resolved()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenFirst<
                    ExceptionCommandGeneric,
                    SimpleCommandOneGeneric,
                    SimpleCommandTwoGeneric
                >()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(10, SimplePromise.result);
        }


        [Test]
        public void race_is_resolved_when_promise_is_first_resolved_first()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).ThenRace<
                SimpleAsyncCommandOneGeneric,
                SimpleAsyncCommandSecondGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }

        [Test]
        public void race_is_resolved_continue_with()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenRace<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void race_is_resolved_when_promise_is_rejected_firstly()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenRace<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseOne.Reject(new Exception());
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }


        [Test]
        public void race_is_resolved_when_promise_is_rejected_next()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenRace<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);
            SimplePromise.simulatePromiseOne.Reject(new Exception());

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_promise_is_resolved_first()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).ThenAny<
                SimpleAsyncCommandOneGeneric,
                SimpleAsyncCommandSecondGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(18, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_continue_with()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenAny<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_promise_is_rejected_first()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenAny<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseOne.Reject(new Exception());
            SimplePromise.simulatePromiseSecond.Resolve(3);

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void any_is_resolved_when_promise_is_rejected_next()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenAny<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            SimplePromise.simulatePromiseOne.Reject(new Exception());

            Assert.AreEqual(22, SimplePromise.result);
        }

        [Test]
        public void get_same_instance_from_commandBinder()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var objects = binding.Value as object[];

            var itemB = ((CommandPromise<int>)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise<int>)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }

        [Test]
        public void release_command_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var objects = binding.Value as object[];
            var commandPromise = (CommandPromise<int>)objects[1];
            var itemA = commandPromise.Test_GetCommand();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(false, itemA.IsRetain);
        }

        [Test]
        public void release_promise_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var objects = binding.Value as object[];
            var commandPromise = (CommandPromise<int>)objects[1];

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(false, commandPromise.IsRetain);
        }
        
        [Test]
        public void return_instance_to_pool_by_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.InstanceCount);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            Assert.AreEqual(4, promisePool.Available);
        }
    }
}