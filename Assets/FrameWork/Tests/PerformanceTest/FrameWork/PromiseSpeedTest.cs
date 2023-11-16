using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Cr7Sund.Framework.Tests
{
    public class PromiseSpeedTest
    {
        public const int warmupCount = 5;
        public const int iterationCount = 10;
        public const int executeCount = 25;
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;
        private IPromiseCommandBinder promiseBinder;

        [SetUp]
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            promiseBinder = new PromiseCommandBinder();
            injectionBinder.Injector.Inject(promiseBinder);
        }

        [Test, Performance]
        public void CreatePromiseCommand()
        {
            var promiseBinding = new CommandPromise();
            Measure.Method(() =>
            {
                promiseBinding
                 .Then<SimplePromiseCommandOne>();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void CreateDelegateCommand()
        {
            var promise = new Promise();
            Measure.Method(() =>
            {
                var command = new SimplePromiseCommandOne();
                promise.Then(command.OnExecute, command.OnCatch, command.OnProgress);

            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void RunPromiseCommand()
        {
            Measure.Method(() =>
            {

                var promiseBinding = new CommandPromise();

                promiseBinding
                 .Then<SimplePromiseCommandOne>();
                promiseBinding.Resolve();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void RunDelegateCommand()
        {
            Measure.Method(() =>
            {
                var promise = new Promise();

                var command = new SimplePromiseCommandOne();
                promise.Then(command.OnExecute, command.OnCatch, command.OnProgress);
                promise.Resolve();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
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
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void ChainDelegateCommand()
        {
            Measure.Method(() =>
            {
                var promise = new Promise();
                SimplePromise.simulatePromise = new Promise();

                var command1 = new SimplePromiseCommandOne();
                var command2 = new SimplePromiseCommandTwo();
                promise
                    .Then(command1.OnExecute, command1.OnCatch, command1.OnProgress)
                    .Then(command2.OnExecute, command2.OnCatch, command2.OnProgress);
                promise.Resolve();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void chain_delegate_command_async()
        {
            Measure.Method(() =>
                       {

                           var promise = new Promise();
                           SimplePromise.simulatePromise = new Promise();

                           var command1 = new SimplePromiseCommandTwo();
                           var command2 = new SimpleAsyncPromiseCommandOne();
                           var command3 = new SimplePromiseCommandOne();
                           promise
                                .Then(command1.OnExecute, command1.OnCatch, command1.OnProgress)
                                .Then(command2.OnExecute, command2.OnCatch, command2.OnProgress)
                                .Then(command3.OnExecute, command3.OnCatch, command3.OnProgress);

                           promise.Resolve();

                           SimplePromise.simulatePromise.Resolve();
                       })
                       .WarmupCount(warmupCount)
                       .MeasurementCount(executeCount)
                       .IterationsPerMeasurement(iterationCount)
                       .GC()
                       .Run();
        }


        [Test, Performance]
        public void chain_promise_command_async()
        {
            Measure.Method(() =>
                       {
                           var promiseBinding = new CommandPromise();
                           SimplePromise.simulatePromise = new Promise();

                           promiseBinding
                            .Then<SimplePromiseCommandTwo>()
                            .Then<SimpleAsyncPromiseCommandOne>()
                            .Then<SimplePromiseCommandOne>();

                           promiseBinding.Resolve();
                           SimplePromise.simulatePromise.Resolve();
                       })
                       .WarmupCount(warmupCount)
                       .MeasurementCount(executeCount)
                       .IterationsPerMeasurement(iterationCount)
                       .GC()
                       .Run();
        }
    }
}
