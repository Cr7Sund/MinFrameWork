using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
{
	[TestFixture]
	public class EventQueueTests
	{
		[Test]
		public void EventQueue_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListen<SampleTestEvent>(() => bus.Raise(new ClassTestEvent()));
				var listener2 = bus.TestListen<SampleTestEvent>();
				var listener3 = bus.TestListen<ClassTestEvent>();

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(), listener1, listener2, listener3);
			}
		}

		[Test]
		public void RaiseImmediately_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListen<SampleTestEvent>(() => bus.RaiseImmediately(new ClassTestEvent()));
				var listener2 = bus.TestListen<SampleTestEvent>();
				var listener3 = bus.TestListen<ClassTestEvent>();

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(), listener1, listener3, listener2);
			}
		}
	}
}