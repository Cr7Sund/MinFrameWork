using Cr7Sund.EventBus.Tests;
using NUnit.Framework;

namespace Cr7Sund.EventBus.Tests
{
	[TestFixture]
	public class AllocationTests
	{
		[Test]
		public void RaisingEvent_DoesNotAllocate()
		{
			var bus = new TestEventBus();
			bus.TestListen<SampleTestEvent>();
			bus.TestListen<SampleTestEvent>();
			bus.TestListen<SampleTestEvent>();

			EventAssert.AssertDoesNotAllocate(() =>
			{
				bus.Raise(new SampleTestEvent());
			});
		}
	}
}