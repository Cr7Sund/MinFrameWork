using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
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