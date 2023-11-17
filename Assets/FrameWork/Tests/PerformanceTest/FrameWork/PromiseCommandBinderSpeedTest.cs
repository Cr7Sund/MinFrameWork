using System;
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

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

          
        }



        [Test, Performance]
        public void ChainPromiseCommand()
        {
            Measure.Method(() =>
            {
                var promiseBinding = new CommandPromise();

                promiseBinding
                 .Then<SimplePromiseCommandOne>()
                 .Then<SimplePromiseCommandTwo>();
                promiseBinding.Resolve();
            })
            .WarmupCount(PromiseSpeedTest.warmupCount)
            .MeasurementCount(PromiseSpeedTest.executeCount)
            .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void ChainBinderCommand()
        {
            Measure.Method(() =>
            {
                var binder = new PromiseCommandBinder();
                injectionBinder.Injector.Inject(binder);

                binder.Bind(SomeEnum.TWO)
                 .Then<SimplePromiseCommandOne>()
                 .Then<SimplePromiseCommandTwo>();

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
