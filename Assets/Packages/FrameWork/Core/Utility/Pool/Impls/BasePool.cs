
namespace Cr7Sund.FrameWork.Util
{
    public abstract class BasePool : IBasePool
    {
        public const int Default_POOL_MAX_COUNT = 1024;
        private const int DefaultCapacity = 1;
        private const int DefaultDoubleCapacity = 4;

        protected int _instanceCount;
        protected int _initSize;


        public IInstanceProvider InstanceProvider { get; set; }
        public virtual int Available { get; }
        public PoolOverflowBehavior OverflowBehavior { get; set; }
        public PoolInflationType InflationType { get; set; }
        public abstract int Count { get; }
        public int MaxCount { get; set; }
        public int TotalLength => _instanceCount;
        public bool IsRetain => Available < _instanceCount;


        public BasePool()
        {
            _initSize = 0;
            MaxCount = Default_POOL_MAX_COUNT;

            OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
            InflationType = PoolInflationType.DOUBLE;
        }

        #region IPool Implementation


        public void SetSize(int size)
        {
            _initSize = size;
        }

        protected int NewInstanceToCreate()
        {
            int instancesToCreate = 0;

            // New fixed-size pool. Populate
            if (_initSize > 0)
            {
                //Illegal overflow. Report and return null
                if (_instanceCount > 0)
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
                instancesToCreate = _instanceCount == 0 ?
                             (InflationType == PoolInflationType.DOUBLE ? DefaultDoubleCapacity : DefaultCapacity) :
                             (InflationType == PoolInflationType.DOUBLE ? _instanceCount : 1);

            }

            return instancesToCreate;
        }

        protected void IncreaseInstance()
        {
            AssertUtil.LessOrEqual(_instanceCount, MaxCount, PoolExceptionType.OVERFLOW);
            _instanceCount++;
        }

        protected void DecreaseInstance()
        {
            _instanceCount--;
        }


        #endregion

        #region IPoolable Implementation

        public void Restore()
        {

        }

        public void Retain()
        {
        }

        public void Release()
        {

            Dispose();
        }
        #endregion

        public virtual void Dispose()
        {
            _initSize = 0;
            _instanceCount = 0;
        }
    }


}
