using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public abstract class BasePoolable : IPoolable
    {
        public bool IsRetain { get; private set; }

        public virtual void Release()
        {
            IsRetain = false;
        }

        public virtual void Restore()
        {
        }

        public virtual void Retain()
        {
            IsRetain = true;
        }
    }
}