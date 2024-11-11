using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
using Unity.PerformanceTesting;
namespace Cr7Sund.PackageTest.Log
{
    public class LogSpeedTest
    {
        public const int warmupCount = 5;
        public const int iterationCount = 10;
        public const int executeCount = 25;

        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }

        [Test]
        [Performance]
        public void TestUnityLog()
        {
            Measure.Method(() =>
                {
                    UnityEngine.Debug.LogWarning("hello message");
                })
                .WarmupCount(warmupCount)
                .MeasurementCount(executeCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void TestSerialLog()
        {
            Measure.Method(() =>
                {
                    Console.Warn("hello Msg");
                })
                .WarmupCount(warmupCount)
                .MeasurementCount(executeCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void TestUnityLogWithFormat()
        {
            string message = "world";
            Measure.Method(() =>
                {
                    UnityEngine.Debug.LogWarning($"hello {message}");
                })
                .WarmupCount(warmupCount)
                .MeasurementCount(executeCount)
                .GC()
                .Run();
        }

        [Test]
        [Performance]
        public void TestSerialLogWithFormat()
        {
            string message = "world";
            Measure.Method(() =>
                {
                    Console.Warn("hello {Msg}", message);
                })
                .WarmupCount(warmupCount)
                .MeasurementCount(executeCount)
                .GC()
                .Run();
        }
    }
}