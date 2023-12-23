
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.EventBus
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