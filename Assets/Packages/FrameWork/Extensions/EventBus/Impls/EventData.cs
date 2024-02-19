using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.EventBus.Api;

namespace Cr7Sund.PackageTest.EventBus.Impl
{
    public abstract class EventData : BasePoolable, IEventData
    {
        public override void Restore()
        {
            Clear();
        }

        public abstract void Clear();
    }
}