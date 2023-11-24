using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using Unity.PerformanceTesting;
namespace Cr7Sund.Framework.Tests
{
    public class PromiseCommandBinderSpeedTest
    {
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;

        [SetUp]
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();
            injectionBinder.Bind<ICommandBinder>().To<CommandBinder>().AsSingleton();
            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);


        }



        [Test]
        [Performance]
        public void ChainPromiseCommand()
        {
            Measure.Method(() =>
                {
                    var promiseBinding = new CommandPromise();

                    promiseBinding
                        .Then<SimpleCommandOne>()
                        .Then<SimpleCommandTwo>();
                    promiseBinding.Resolve();
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void ChainBinderCommand()
        {

            Measure.Method(() =>
                {
                    var binder = new CommandPromiseBinder();
                    injectionBinder.Injector.Inject(binder);

                    binder.Bind(SomeEnum.TWO)
                        .Then<SimpleCommandOne>()
                        .Then<SimpleCommandTwo>();

                    binder.ReactTo(SomeEnum.TWO);
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void ChainBinderCommand_Pool()
        {
            Measure.Method(() =>
                {
                    var binder = new CommandPromiseBinder();
                    injectionBinder.Injector.Inject(binder);
                    
                    binder.Bind(SomeEnum.TWO).AsPool().AsOnce()
                        .Then<SimpleCommandOne>()
                        .Then<SimpleCommandTwo>();

                    binder.ReactTo(SomeEnum.TWO);
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void ReactBinderCommand()
        {
            var binder = new CommandPromiseBinder();
            injectionBinder.Injector.Inject(binder);
                    
            binder.Bind(SomeEnum.TWO)
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();
            
            Measure.Method(() =>
                {
                    binder.ReactTo(SomeEnum.TWO);
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
                .GC()
                .Run();
        }


    }
}
