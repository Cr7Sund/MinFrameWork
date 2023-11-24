using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
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
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            object[] objects = binding.Value as object[];

            var itemB = ((CommandPromise)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }

        [Test]
        public void release_promise_after_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool().AsOnce()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            object[] objects = binding.Value as object[];
            var commandPromise = (CommandPromise)objects[1];

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            Assert.AreEqual(false, commandPromise.IsRetain);
        }

        [Test]
        public void get_same_command_from_commandBinder()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            object[] objects = binding.Value as object[];

            var itemB = ((CommandPromise)objects[3]).Test_GetCommand();
            var itemA = ((CommandPromise)objects[1]).Test_GetCommand();


            Assert.AreEqual(itemA, itemB);
        }


        [Test]
        public void return_instance_to_pool_by_resolved()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool().AsOnce()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.InstanceCount);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(4, promisePool.Available);
        }

        [Test]
        public void return_instance_to_pool_by_rejected()
        {
            var binding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool().AsOnce()
                .Then<SimpleCommandOne>()
                .Then<ExceptionCommand>()
                .Then<SimpleCommandOne>();

            var promisePool = poolBinder.GetOrCreate<CommandPromise>();
            Assert.AreEqual(0, promisePool.Available);
            Assert.AreEqual(4, promisePool.InstanceCount);

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(4, promisePool.Available);
        }

        [Test]
        public void react_same_promise_multiple_times()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(16 * 3, SimplePromise.result);

            SimplePromise.result = 0;
            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(16 * 3, SimplePromise.result);
        }

        [Test]
        public void react_exception_multiple_times_but_asOnce()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool().AsOnce()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            var ex = Assert.Throws<PromiseException>(() =>
            {
                _commandPromiseBinder.ReactTo(SomeEnum.ONE);
                Assert.AreEqual(22 * 3, SimplePromise.result);
            });
            Assert.AreEqual(PromiseExceptionType.CAN_NOT_REACT_RELEASED, ex.Type);
        }

        [Test]
        public void react_exception_in_running_promise()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool();

            promiseBinding
                .Then<SimpleCommandTwo>()
                .Then<SimpleAsyncCommandOne>()
                .Then<SimpleCommandOne>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            var ex = Assert.Throws<PromiseException>(() =>
                _commandPromiseBinder.ReactTo(SomeEnum.ONE));
            Assert.AreEqual(PromiseExceptionType.CAN_NOT_REACT_RUNNING, ex.Type);
        }

        [Test]
        public void stop_first_start_new_stop_async_operation()
        {
            var promiseBinding = _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool();

            promiseBinding
                .Then<SimpleCommandTwo>()
                .Then<SimpleAsyncCommandOne>()
                .Then<SimpleCommandOne>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);

            promiseBinding.RestartPromise();
            SimplePromise.result = 0;

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            SimplePromise.simulatePromise.Resolve();

            Assert.AreEqual((((0 + 2) * 3 + 3) * 5 + 1) * 2, SimplePromise.result);
        }

        [Test]
        public void exception_in_bind_duplicate()
        {
            _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                .Then<SimpleCommandTwo>()
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();

            _commandPromiseBinder.ReactTo(SomeEnum.ONE);
            Assert.AreEqual(16 * 3, SimplePromise.result);

            TestDelegate testDelegate = delegate
            {
                _commandPromiseBinder.Bind(SomeEnum.ONE).AsPool()
                    .Then<SimpleCommandTwo>();
            };
            var ex = Assert.Throws<BinderException>(testDelegate);
            Assert.That(ex.Type == BinderExceptionType.CONFLICT_IN_BINDER);
        }
    }
}
