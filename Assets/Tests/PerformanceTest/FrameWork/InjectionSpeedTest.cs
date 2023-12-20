using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using Unity.PerformanceTesting;
namespace Cr7Sund.Framework.Tests
{
    public class InjectionSpeedTest
    {
        private const int Count = 100;
        private IInjectionBinder injectionBinder;

        [SetUp]
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();
            injectionBinder.Bind<int>().To(42);
            injectionBinder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
        }

        [Test]
        [Performance]
        public void TestInjectGetInstance()
        {
            Measure.Method(() =>
                {
                    injectionBinder.GetInstance<InjectableSuperClass>();
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void TestNewGetInstance()
        {
            Measure.Method(() =>
                {
                    var t = new InjectableDerivedClass();
                    t.injected = new ClassToBeInjected();
                })
                .WarmupCount(PromiseSpeedTest.warmupCount)
                .MeasurementCount(PromiseSpeedTest.executeCount)
                .GC()
                .Run();
        }
    }
}
