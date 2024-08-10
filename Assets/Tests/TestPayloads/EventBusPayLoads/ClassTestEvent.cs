using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.PackageTest.EventBus
{
	public class ClassTestEvent : BasePoolable, IEventData
    {
		public float Data;

		public int RecursionDepth { get; set; }



    }
}