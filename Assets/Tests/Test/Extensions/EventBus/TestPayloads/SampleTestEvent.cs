using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.EventBus.Tests
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