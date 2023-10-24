using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
	public class TestContext
	{
		object view;

		[SetUp]
		public void SetUp()
		{
			Context.FirstContext = null;
			view = new object();
		}

		[Test]
		public void TestContextIsFirstContext()
		{
			Context context = new Context (view);
			Assert.AreEqual (context, Context.FirstContext);
		}

		[Test]
		public void TestContextView()
		{
			Context context = new Context (view);
			Assert.AreEqual (view, context.contextView);
		}

		[Test]
		public void TestAutoStartup()
		{
			TestContextSubclass context = new TestContextSubclass (view);
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptMapping()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_MAPPING);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptLaunch()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_LAUNCH);
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptAll()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_MAPPING | ContextStartupFlags.MANUAL_LAUNCH);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestOldStyleInterruptLaunch()
		{
			TestContextSubclass context = new TestContextSubclass (view, false);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestOldStyleAutoStartup()
		{
			TestContextSubclass context = new TestContextSubclass (view, true);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}
	}


}
