using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
namespace Cr7Sund.Framework.PromiseCommandTest
{

    public class PromiseCommandBinderNoGenericTests
    {
        private ICommandPromiseBinder _commandPromiseBinder;
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;

        [SetUp]
        public void Setup()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);
            injectionBinder.Bind<ICommandBinder>().To(new CommandBinder());

            _commandPromiseBinder = new CommandPromiseBinder();
            ((CommandPromiseBinder)_commandPromiseBinder).UsePooling = false;

            injectionBinder.Injector.Inject(_commandPromiseBinder);

            SimplePromise.simulatePromiseOne = new Promise<int>();
            SimplePromise.simulatePromiseSecond = new Promise<int>();
            SimplePromise.simulatePromise = new Promise();

            TestMono.SimplePromise.result = 0;
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
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.Then<SimpleCommandTwo>().Then<SimpleCommandOne>().Then<SimpleCommandTwo>()
                ;


            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            Assert.AreEqual(16 * 3, SimplePromise.result);
        }

        [Test]
        public void command_binder_simple_check_pool()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandTwo>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise<int>>();
            var promiseNoValuePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(0, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(0, promiseNoValuePool.Available);
            Assert.AreEqual(0, promisePool.Count);
            Assert.AreEqual(0, promiseNoValuePool.Count);
        }

        [Test]
        public void command_binder_release_done()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            promiseBinding.Then<SimpleCommandTwo>()
            .Then<SimpleCommandOne>()
            .Then<SimpleCommandTwo>();


            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
        }

        [Test]
        public void command_binder_with_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            promiseBinding.Then<SimpleCommandTwo>().Then<SimpleAsyncCommandOne>().Then<SimpleCommandOne>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(6, SimplePromise.result);

            SimplePromise.simulatePromise.Resolve();

            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
        }


        [Test]
        public void command_injection()
        {
            injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();

            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            var binding = promiseBinding.Then<TestInjectionCommand>()
                ;


            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(1, SimplePromise.result);
        }

        [Test]
        public void get_same_instance_from_commandBinder()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            var objects = binding.Value;

            var itemB = ((CommandPromise)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }

        [Test]
        public void release_promise_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            var objects = binding.Value;
            var commandPromise = (CommandPromise)objects[1];

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            Assert.AreEqual(false, commandPromise.IsRetain);
        }

        [Test]
        public void get_same_command_from_commandBinder()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            var objects = binding.Value;

            var itemB = ((CommandPromise)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }


        [Test]
        public void return_instance_to_pool_by_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            IPool<CommandPromise> promisePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(8, promisePool.Available);
            Assert.AreEqual(8, promisePool.Count);
        }

        [Test]
        public void return_instance_to_pool_by_rejected()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandOne>()
                .Then<ExceptionCommand>()
                .Then<SimpleCommandOne>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.Count);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            binding.Dispose();
            Assert.AreEqual(8, promisePool.Available);
            Assert.AreEqual(8, promisePool.Count);
        }

        [Test]
        public void react_same_promise_multiple_times()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandOne>();

            SimplePromise.result = 0;
            for (int i = 0; i < 5; i++)
            {
                var startValue = SimplePromise.result;
                _commandPromiseBinder.ReactTo(SomeEnum.ONE);
                Assert.AreEqual((startValue + 1) * 2, SimplePromise.result);
            }
        }

        [Test]
        public void react_exception_multiple_times_but_asOnce()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            var ex = Assert.Throws<Util.MyException>(() =>
            {
                _commandPromiseBinder.ReactTo(SomeEnum.ONE);
                Assert.AreEqual(22 * 3, SimplePromise.result);
            });
            Assert.AreEqual(PromiseExceptionType.CAN_NOT_REACT_RELEASED, ex.Type);
        }

        [Test]
        public void react_exception_in_running_promise()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);

            promiseBinding
                .Then<SimpleCommandTwo>()
                .Then<SimpleAsyncCommandOne>()
                .Then<SimpleCommandOne>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            var ex = Assert.Throws<Util.MyException>(() =>
                _commandPromiseBinder.ReactTo(SomeEnum.ONE));
            Assert.AreEqual(PromiseExceptionType.CAN_NOT_REACT_RUNNING, ex.Type);
        }

        [Test]
        public void react_multiple_bindings()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsOnce()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            _commandPromiseBinder.Bind(SomeEnum.TWO).AsOnce()
                  .Then<SimpleCommandTwo>()
                  .Then<SimpleCommandOne>()
                  .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.TWO);
        }

        [Test]
        public void pooling_binding_multiple_times()
        {
            ((CommandPromiseBinder)_commandPromiseBinder).UsePooling = true;
            for (int i = 0; i < 5; i++)
            {
                SimplePromise.result = 0;

                _commandPromiseBinder.Bind(SomeEnum.ONE)
                    .Then<SimpleCommandOne>().AsOnce();
                _commandPromiseBinder.ReactTo(SomeEnum.ONE);
                Assert.AreEqual((0 + 1) * 2, SimplePromise.result);

                _commandPromiseBinder.Unbind(SomeEnum.ONE);
                var bindingPool = poolBinder.GetOrCreate<CommandPromiseBinding>();
                Assert.AreEqual(1, bindingPool.Available);
                Assert.AreEqual(1, bindingPool.Count);
            }
        }

        [Test]
        public void pooling_binding_release()
        {
            SimplePromise.result = 0;
            ((CommandPromiseBinder)_commandPromiseBinder).UsePooling = true;
            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.Bind(i)
                    .Then<SimpleCommandOne>().AsOnce();
                _commandPromiseBinder.ReactTo(i);
            }

            for (int i = 0; i < 5; i++)
            {
                _commandPromiseBinder.Unbind(i);
            }

            var bindingPool = poolBinder.GetOrCreate<CommandPromiseBinding>();
            Assert.AreEqual(8, bindingPool.Available);
            Assert.AreEqual(8, bindingPool.Count);
        }


        [Test]
        public void stop_first_start_new_stop_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE);
            promiseBinding
                .Then<SimpleCommandTwo>()
                .Then<SimpleAsyncCommandOne>()
                .Then<SimpleCommandOne>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            SimplePromise.result = 0;

            promiseBinding.RestartPromise();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            SimplePromise.simulatePromise.Resolve();

            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void exception_in_bind_duplicate()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE)
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(16 * 3, SimplePromise.result);

            TestDelegate testDelegate = delegate
            {
                _commandPromiseBinder.Bind(SomeEnum.ONE)
                    .Then<SimpleCommandTwo>();
            };
            var ex = Assert.Throws<Util.MyException>(testDelegate);
            Assert.AreEqual(BinderExceptionType.CONFLICT_IN_BINDER, ex.Type);
        }
    }
}
