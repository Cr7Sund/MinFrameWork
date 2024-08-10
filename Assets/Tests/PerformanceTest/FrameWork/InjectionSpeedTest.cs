using Cr7Sund.IocContainer;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
using Unity.PerformanceTesting;
namespace Cr7Sund.PackageTest.IOC
{
    public class PromiseSpeedTest
    {
        public const int warmupCount = 5;
        public const int iterationCount = 10;
        public const int executeCount = 25;
    }

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
