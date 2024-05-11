using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.PackageTest.EventBus
{
    public struct SampleTestEvent : IEventData
    {
		public int Data;

		public int RecursionDepth { get; set; }

        public bool IsRetain => throw new System.NotImplementedException();

        public void Release()
        {
        }

        public void Restore()
        {
        }

        public void Retain()
        {
        }
    }
}