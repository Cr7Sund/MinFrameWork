using Cr7Sund.Framework.Impl;

namespace Cr7Sund.EventBus.Tests
{
	public class ClassTestEvent : BasePoolable, IEventData
    {
		public float Data;

		public int RecursionDepth { get; set; }



    }
}