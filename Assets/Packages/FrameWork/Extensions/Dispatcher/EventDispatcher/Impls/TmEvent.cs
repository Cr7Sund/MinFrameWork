using Cr7Sund.Framework.Api;
namespace Cr7Sund.Framework.Impl
{
    public class TmEvent : IEvent, IPoolable
    {

        protected int retainCount;

        public TmEvent()
        {
        }

        /// Construct a TmEvent
        public TmEvent(object type, IEventDispatcher target, object data)
        {
            Type = type;
            Target = target;
            Data = data;
        }
        public object Type { get; set; }
        public IEventDispatcher Target { get; set; }
        public object Data { get; set; }

        #region IPoolable implementation
        public bool IsRetain
        {
            get
            {
                return retainCount > 0;
            }
        }

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
            Type = null;
            Target = null;
            Data = null;
        }
        #endregion
    }

}
