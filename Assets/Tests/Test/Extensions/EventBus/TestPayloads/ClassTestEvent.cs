using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.EventBus.Api;

namespace Cr7Sund.PackageTest.EventBus
{
	public class ClassTestEvent : BasePoolable, IEventData
    {
		public float Data;

		public int RecursionDepth { get; set; }



    }
}