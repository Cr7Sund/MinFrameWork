using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.TestTools;
namespace Cr7Sund.Framework.PromiseCommandTest
{
    public class PromiseCommandBinderTests
    {
        private ICommandPromiseBinder<int> _commandPromiseBinder;
        private ICommandBinder commandBinder;
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;

        [SetUp]
        public void Setup()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();
            commandBinder = new CommandBinder();
            
            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);
            injectionBinder.Bind<ICommandBinder>().To(commandBinder);
            injectionBinder.Bind<IInternalLog>().To<InternalLogger>();

            _commandPromiseBinder = new CommandPromiseBinder<int>();
            ((CommandPromiseBinder<int>)_commandPromiseBinder).UsePooling = false;

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

            promiseBinding.Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(16 * 3, SimplePromise.result);
        }

        [Test]
        public void command_binder_simple_check_pool()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOneGeneric>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            var promiseNoValuePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(0, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Count);
        }

        [Test]
        public void command_binder_simple_asOnce()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                ;

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(16 * 3, SimplePromise.result);
        }

        [Test]
        public void command_binder_simple_asOnce_asPool()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                ;

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            Assert.AreEqual(16 * 3, SimplePromise.result);
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

            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
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

            Assert.AreEqual(((0 + 1 + 3) * 5 + 2 + 1) * 2, SimplePromise.result);
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
            LogAssert.Expect(UnityEngine.LogType.Error, new Regex("System.NotImplementedException"));

            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .ThenRace<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 5);
            SimplePromise.simulatePromiseOne.Reject(new NotImplementedException());
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
        public void react_any_promise_multiple_times()
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

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 2);
            Assert.AreEqual(26, SimplePromise.result);
        }


        [Test]
        public void react_any_promise_as_pool()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .ThenAny<
                    SimpleAsyncCommandOneGeneric,
                    SimpleAsyncCommandSecondGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            var promiseNoValuePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(3, promisePool.Available);
            Assert.AreEqual(8, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            Assert.AreEqual(2, promisePool.Available);
            Assert.AreEqual(8, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(4, promiseNoValuePool.Count);

            SimplePromise.simulatePromiseSecond.Resolve(3);
            SimplePromise.simulatePromiseOne.Reject(new Exception());
            Assert.AreEqual(8, promisePool.Available);
            Assert.AreEqual(4, promiseNoValuePool.Available);
            Assert.AreEqual(8, promisePool.Count);
            Assert.AreEqual(4, promiseNoValuePool.Count);
        }

        [Test]
        public void release_any_promise_as_pool_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                 .ThenAny<
                     SimpleAsyncCommandOneGeneric,
                     SimpleAsyncCommandSecondGeneric>()
                 .Then<SimpleCommandOneGeneric>();

            ISemiBinding objects = binding.Value;
            List<ICommandPromise<int>> testGetPromiseList = ((CommandPromiseBinding<int>)binding).Test_GetPromiseList();
            var promiseList = new List<ICommandPromise<int>>(testGetPromiseList);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            SimplePromise.simulatePromiseSecond.Resolve(3);

            for (int i = 0; i < objects.Count; i++)
            {
                object item = objects[i];
                var commandPromise = (CommandPromise<int>)item;
                Assert.AreEqual(false, commandPromise.IsRetain);
            }

            foreach (var commandPromise in promiseList)
            {
                Assert.AreEqual(false, commandPromise.IsRetain);
            }
        }

        [Test]
        public void get_same_command_from_commandBinder()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var objects = binding.Value;

            var itemB = ((CommandPromise<int>)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise<int>)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }

        [Test]
        public void release_promise_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var objects = binding.Value;

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);

            for (int i = 0; i < objects.Count; i++)
            {
                object item = objects[i];
                var commandPromise = (CommandPromise<int>)item;
                Assert.AreEqual(false, commandPromise.IsRetain);
            }
        }

        [Test]
        public void return_instance_to_pool_by_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            var promiseNoValuePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Count);
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            Assert.AreEqual(1, promiseNoValuePool.Available);
            Assert.AreEqual(1, promiseNoValuePool.Count);
            Assert.AreEqual(4, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);
        }

        [Test]
        public void return_instance_to_pool_by_rejected()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, new Regex("System.NotImplementedException"));

            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOneGeneric>()
                .Then<ExceptionCommandGeneric>()
                .Then<SimpleCommandOneGeneric>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            var promiseNoValuePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Count);
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 1);
            Assert.AreEqual(1, promiseNoValuePool.Available);
            Assert.AreEqual(1, promiseNoValuePool.Count);
            Assert.AreEqual(4, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);
        }

        [Test]
        public void react_same_promise_multiple_times()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOneGeneric>();

            SimplePromise.result = 0;
            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);
                Assert.AreEqual((0 + 1) * 2, SimplePromise.result);
            }
        }

        [Test]
        public void pooling_binding_multiple_times()
        {
            SimplePromise.result = 0;
            ((CommandPromiseBinder<int>)_commandPromiseBinder).UsePooling = true;
            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.Bind(SomeEnum.ONE)
                    .Then<SimpleCommandOneGeneric>().AsOnce();
                _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);
                Assert.AreEqual((0 + 1) * 2, SimplePromise.result);

                _commandPromiseBinder.Unbind(SomeEnum.ONE);
                var bindingPool = poolBinder.GetOrCreate<CommandPromiseBinding<int>>();
                Assert.AreEqual(1, bindingPool.Available);
                Assert.AreEqual(1, bindingPool.Count);
            }
        }

        [Test]
        public void pooling_binding_release()
        {
            SimplePromise.result = 0;
            ((CommandPromiseBinder<int>)_commandPromiseBinder).UsePooling = true;
            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.Bind(i)
                    .Then<SimpleCommandOneGeneric>().AsOnce();
                _commandPromiseBinder.ReactTo(i, 0);
            }

            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.Unbind(i);
            }

            var bindingPool = poolBinder.GetOrCreate<CommandPromiseBinding<int>>();
            Assert.AreEqual(8, bindingPool.Available);
            Assert.AreEqual(8, bindingPool.Count);
        }


        [Test]
        public void stop_first_start_new_stop_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            promiseBinding
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleAsyncCommandOneGeneric>()
                .Then<SimpleCommandOneGeneric>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);

            promiseBinding.RestartPromise();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);
            SimplePromise.simulatePromiseOne.Resolve(0);

            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void exception_in_bind_duplicate()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandTwoGeneric>()
                .Then<SimpleCommandOneGeneric>()
                .Then<SimpleCommandTwoGeneric>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE, 0);
            Assert.AreEqual(16 * 3, SimplePromise.result);

            TestDelegate testDelegate = delegate
            {
                _commandPromiseBinder.Bind(SomeEnum.ONE)
                    .Then<SimpleCommandTwoGeneric>();
            };
            var ex = Assert.Throws<Util.MyException>(testDelegate);
            Assert.AreEqual(BinderExceptionType.CONFLICT_IN_BINDER, ex.Type);
        }
    }
}
