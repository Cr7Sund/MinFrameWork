using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
{
	[TestFixture]
	public class MultipleEventListenersTests
	{
		[Test]
		public void MultipleListeners_SubscribeOrder_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListen<SampleTestEvent>();
				var listener2 = bus.TestListen<SampleTestEvent>();

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(), listener1, listener2);
			}
		}

		[Test]
		public void ListenerPriority_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListen<SampleTestEvent>(-10);
				var listener2 = bus.TestListen<SampleTestEvent>(100);
				var listener3 = bus.TestListen<SampleTestEvent>(-5);

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(), listener2, listener3, listener1);
			}
		}

		[Test]
		public void RemovingAndAddingListenersDuringRaise_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListener<SampleTestEvent>();
				var listener2 = bus.TestListener<SampleTestEvent>();
				var listener3 = bus.TestListener<SampleTestEvent>();
				var listener4 = bus.TestListener<SampleTestEvent>();
				var listener5 = bus.TestListener<SampleTestEvent>();
				var listener6 = bus.TestListener<SampleTestEvent>();
				var listener7 = bus.TestListener<SampleTestEvent>();

				listener1.Subscribe();

				listener2.Subscribe(() =>
					listener1.Unsubscribe()); // Unsubscribing previous listener is shouldn't affect this raise.

				listener3.Subscribe(() =>
					listener1.Subscribe(10)); // Adding a listener to the front shouldn't affect this raise.

				listener4.Subscribe(() =>
					listener6.Unsubscribe()); // Unsubscribing an upcoming listener should make it not receive the event.

				listener5.Subscribe(() => listener7.Subscribe()); // Adding a listener to the end should make it receive the event.

				listener6.Subscribe();

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(), new[]
				{
					listener1, listener2, listener3, listener4, listener5, listener6, listener7
				}, new[]
				{
					listener1, listener2, listener3, listener4, listener5, listener7
				});
			}
		}
	}
}