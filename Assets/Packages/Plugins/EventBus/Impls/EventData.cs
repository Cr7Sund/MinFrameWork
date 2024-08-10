using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.Package.EventBus.Impl
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