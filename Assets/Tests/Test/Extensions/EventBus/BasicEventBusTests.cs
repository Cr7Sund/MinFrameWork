﻿using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
{
	[TestFixture]
	public class BasicEventBusTests
	{
		[Test]
		public void SubscribeRaiseUnsubscribeRaise_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener = bus.TestListener<SampleTestEvent>();

				listener.Subscribe();
				listener.AssertDidNotReceive();
				bus.Raise(new SampleTestEvent());
				listener.AssertDidReceiveAndReset();

				listener.Unsubscribe();
				bus.Raise(new SampleTestEvent());
				listener.AssertDidNotReceive();
			}
		}
	}
}