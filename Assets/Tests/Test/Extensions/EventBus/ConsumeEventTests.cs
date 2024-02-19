using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
{
	[TestFixture]
	public class ConsumeEventTests
	{
		[Test]
		public void ConsumeEvent_Works()
		{
			using (var bus = new TestEventBus())
			{
				var listener1 = bus.TestListen<SampleTestEvent>();
				var listener2 = bus.TestListen<SampleTestEvent>(() => bus.ConsumeCurrentEvent());
				var listener3 = bus.TestListen<SampleTestEvent>();
				var listener4 = bus.TestListen<SampleTestEvent>();

				bus.AssertListenersInvokedInOrder(new SampleTestEvent(),
					new[] { listener1, listener2, listener3, listener4 },
					new[] { listener1, listener2 });
			}
		}
	}
}