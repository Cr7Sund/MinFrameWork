using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Impl;
using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.PackageTest.EventBus
{
	public class ClassTestEvent : BasePoolable, IEventData
    {
		public float Data;

		public int RecursionDepth { get; set; }



    }
}