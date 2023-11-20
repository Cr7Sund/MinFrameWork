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

        [SetUp]
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

        }

        #region  Promise Delegate

        [Test, Performance]
        public void CreateDelegateCommand()
        {
            var promise = new Promise();
            Measure.Method(() =>
            {
                var command = new SimpleCommandOne();
                promise.Then(command.OnExecute, command.OnCatch, command.OnProgress);

            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }
        [Test, Performance]
        public void CreateDelegateGenericCommand()
        {
            var promise = new Promise<int>();
            Measure.Method(() =>
            {
                var command = new SimpleCommandOneGeneric();
                promise.Then(command.OnExecute);

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

                var command = new SimpleCommandOne();
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
        public void ChainDelegateCommand()
        {
            Measure.Method(() =>
            {
                var promise = new Promise();
                SimplePromise.simulatePromise = new Promise();

                var command1 = new SimpleCommandOne();
                var command2 = new SimpleCommandTwo();
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
        public void ChainDelegateCommandAsync()
        {
            Measure.Method(() =>
                       {

                           var promise = new Promise();
                           SimplePromise.simulatePromise = new Promise();

                           var command1 = new SimpleCommandTwo();
                           var command2 = new SimpleAsyncCommandOne();
                           var command3 = new SimpleCommandOne();
                           promise
                                .Then(command1.OnExecute, command1.OnCatch, command1.OnProgress)
                                .Then(command2.OnExecuteAsync, command2.OnCatch, command2.OnProgress)
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
        public void ConvertDelegateCommand()
        {
            Measure.Method(() =>
                       {
                           var promise = new Promise<int>();

                           var command1 = new SimpleCommandTwoGeneric();
                           var command2 = new ConvertCommand();
                           var command3 = new AnotherCommand();
                           promise
                                .Then(command1.OnExecute)
                                .Then(command2.OnExecute)
                                .Then(command3.OnExecute);

                           promise.Resolve(1);
                       })
                        .WarmupCount(PromiseSpeedTest.warmupCount)
                        .MeasurementCount(PromiseSpeedTest.executeCount)
                        .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
                       .GC()
                       .Run();
        }

        #endregion

        #region PromiseCommand

        [Test, Performance]
        public void CreatePromiseCommand()
        {
            var promiseBinding = new CommandPromise();
            Measure.Method(() =>
            {
                promiseBinding
                 .Then<SimpleCommandOne>();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void CreatePromiseGenericCommand()
        {
            var promiseBinding = new CommandPromise<int>();
            Measure.Method(() =>
            {
                promiseBinding
                                .Then<SimpleCommandOneGeneric>();

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
                 .Then<SimpleCommandOne>();
                promiseBinding.Resolve();
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
                 .Then<SimpleCommandOne>()
                 .Then<SimpleCommandTwo>();
                promiseBinding.Resolve();
            })
            .WarmupCount(warmupCount)
            .MeasurementCount(executeCount)
            .IterationsPerMeasurement(iterationCount)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void ChainPromiseCommandAsync()
        {
            Measure.Method(() =>
                       {
                           var promiseBinding = new CommandPromise();
                           SimplePromise.simulatePromise = new Promise();

                           promiseBinding
                            .Then<SimpleCommandTwo>()
                            .Then<SimpleAsyncCommandOne>()
                            .Then<SimpleCommandOne>();

                           promiseBinding.Resolve();
                           SimplePromise.simulatePromise.Resolve();
                       })
                       .WarmupCount(warmupCount)
                       .MeasurementCount(executeCount)
                       .IterationsPerMeasurement(iterationCount)
                       .GC()
                       .Run();
        }

        [Test, Performance]
        public void ConvertPromiseCommand()
        {
            Measure.Method(() =>
            {
                var promiseBinding = new CommandPromise<int>();

                promiseBinding
                .Then<SimpleCommandTwoGeneric>()
                .Then<ConvertCommand, float>()
                .Then<AnotherCommand>();

                promiseBinding.Resolve(0);
            })
            .WarmupCount(PromiseSpeedTest.warmupCount)
            .MeasurementCount(PromiseSpeedTest.executeCount)
            .IterationsPerMeasurement(PromiseSpeedTest.iterationCount)
            .GC()
            .Run();
        }

      

        #endregion
    }
}
