/**
 * Technically this is not a unit test. Rather, it's a
 * development tool to rate the value of the Reflector extension.
 *
 * This scenario measured 64ms with the ReflectionBinder, 302ms without.
 */

using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using System.Diagnostics;
using Unity.PerformanceTesting;

namespace Cr7Sund.Framework.Tests
{
	public class InjectionSpeedTest
	{
		const int Count = 100;
		IInjectionBinder injectionBinder = null;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();
			injectionBinder.Bind<int>().ToValue(42);
			injectionBinder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
		}

		[Test, Performance]
		public void TestInjectGetInstance()
		{
			Measure.Method(() =>
			{
				for (int a = 0; a < Count; a++)
				{
					injectionBinder.GetInstance<InjectableSuperClass>();
				}
			})
			.WarmupCount(5)
			.MeasurementCount(10)
			.GC()
			.Run();
		}

		[Test, Performance]
		public void TestNewGetInstance()
		{
			Measure.Method(() =>
			{
				for (int a = 0; a < Count; a++)
				{
					var t = new InjectableDerivedClass();
					t.injected = new ClassToBeInjected();
				}
			})
			.WarmupCount(5)
			.MeasurementCount(10)
			.GC()
			.Run();
		}
	}
}
