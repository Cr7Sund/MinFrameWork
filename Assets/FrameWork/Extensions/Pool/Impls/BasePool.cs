using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework.Constraints;
namespace Cr7Sund.Framework.Impl
{
    public abstract class BasePool : IBasePool
    {
        protected int _instanceCount;
        protected int _initSize;

        public BasePool()
        {

            _initSize = 0;

            OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
            inflationType = PoolInflationType.DOUBLE;
        }

        #region IPool Implementation
        public IInstanceProvider InstanceProvider { get; set; }

        public virtual int Available { get; }

        public PoolOverflowBehavior OverflowBehavior { get; set; }
        public PoolInflationType inflationType { get; set; }

        public int Count => _instanceCount;

        public void SetSize(int size)
        {
            _initSize = size;
        }


        public virtual void Clean()
        {
            _instanceCount = 0;
        }

        protected int NewInstanceToCreate()
        {
            int instancesToCreate = 0;

            // New fixed-size pool. Populate
            if (_initSize > 0)
            {
                //Illegal overflow. Report and return null
                if (Count > 0)
                {
                    AssertUtil.IsFalse(OverflowBehavior == PoolOverflowBehavior.EXCEPTION, PoolExceptionType.OVERFLOW);
                }
                else
                {
                    instancesToCreate = _initSize;
                }
            }
            else
            {
                if (Count == 0 || inflationType == PoolInflationType.INCREMENT)
                {
                    // 1 or 4 
                    instancesToCreate = 1;
                }
                else
                {
                    instancesToCreate = Count;
                }
            }

            return instancesToCreate;
        }

        #endregion

        #region IPoolable Implementation
        public bool IsRetain { get; private set; }

        public void Restore()
        {
            Clean();
            _initSize = 0;
        }

        public void Retain()
        {
            IsRetain = true;
        }

        public void Release()
        {
            IsRetain = false;
        }
        #endregion
    }


}
