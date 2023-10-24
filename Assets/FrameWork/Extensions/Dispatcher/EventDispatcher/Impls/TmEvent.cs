using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class TmEvent : IEvent, IPoolable
    {
        public object Type { get; set; }
        public IEventDispatcher Target { get; set; }
        public object Data { get; set; }

        protected int retainCount;

        public TmEvent()
        { }

        /// Construct a TmEvent
        public TmEvent(object type, IEventDispatcher target, object data)
        {
            this.Type = type;
            this.Target = target;
            this.Data = data;
        }

        #region IPoolable implementation
        public bool IsRetain => retainCount > 0;

        public void Retain()
        {
            retainCount++;
        }
        public void Release()
        {
            retainCount--;
            if (retainCount == 0)
            {
                Target.ReleaseEvent(this);
            }
        }

        public void Restore()
        {
            this.Type = null;
            this.Target = null;
            this.Data = null;
        }


        #endregion
    }

}