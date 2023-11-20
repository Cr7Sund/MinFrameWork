using System;
using System.Collections;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Impl
{
    public abstract class BasePool : IBasePool
    {
        protected int _instanceCount;
        protected int _size;

        public BasePool()
        {

            _size = 0;

            OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
            inflationType = PoolInflationType.DOUBLE;
        }

        #region IPool Implementation
        public IInstanceProvider InstanceProvider { get; set; }

        public virtual int Available { get; }
        public int InstanceCount => _instanceCount;

        public PoolOverflowBehavior OverflowBehavior { get; set; }
        public PoolInflationType inflationType { get; set; }

        public int Count => _size;

        public void SetSize(int size)
        {
            this._size = size;
        }


        public virtual void Clean()
        {
            _instanceCount = 0;
        }

        protected int NewInstanceToCreate()
        {
            int instancesToCreate = 0;

            // New fixed-size pool. Populate
            if (Count > 0)
            {
                //Illegal overflow. Report and return null
                AssertUtil.IsFalse(InstanceCount > 0 && OverflowBehavior == PoolOverflowBehavior.EXCEPTION,
                                 new PoolException("A pool has overflowed its limit.\n\t", PoolExceptionType.OVERFLOW)
                    );

                instancesToCreate = Count;
            }
            else
            {
                if (InstanceCount == 0 || inflationType == PoolInflationType.INCREMENT)
                {
                    // 1 or 4 
                    instancesToCreate = 1;
                }
                else
                {
                    instancesToCreate = InstanceCount;
                }
            }

            return instancesToCreate;
        }

        #endregion

        #region  IPoolable Implementation

        public bool IsRetain { get; private set; }

        public void Restore()
        {
            Clean();
            _size = 0;
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

    public class Pool : BasePool, IPool
    {
        private HashSet<object> instancesInUse { get; set; }
        public override int Available => instancesAvailable.Count;
        public object Value => throw new System.NotImplementedException();
        public Type poolType { get; set; }

        /// Stack of instances still in the Pool.
        protected Stack instancesAvailable;


        public Pool()
        {
            instancesInUse = new HashSet<object>();
            instancesAvailable = new Stack();
        }

        public override void Clean()
        {
            instancesAvailable.Clear();
            instancesInUse.Clear();
            base.Clean();
        }

        #region  IPool Implementation

        public object GetInstance()
        {
            var instance = GetInstanceInternal();
            if (instance is IPoolable)
            {
                (instance as IPoolable).Retain();
            }
            return instance;
        }

        public void ReturnInstance(object value)
        {
            if (instancesInUse.Contains(value))
            {
                if (value is IPoolable)
                {
                    (value as IPoolable).Restore();
                }
                instancesInUse.Remove(value);
                instancesAvailable.Push(value);
            }
        }

        private void RemoveInstance(object value)
        {
            AssertUtil.IsInstanceOf(poolType, value,
                new PoolException(
                    "Attempt to remove a instance from a pool that is of the wrong Type:\n\t\tPool type: " + poolType.ToString() + "\n\t\tInstance type: " + value.GetType().ToString(),
                    PoolExceptionType.TYPE_MISMATCH));
            if (instancesInUse.Contains(value))
            {
                instancesInUse.Remove(value);
            }
            else
            {
                instancesAvailable.Pop();
            }
        }

        private object GetInstanceInternal()
        {
            if (instancesAvailable.Count > 0)
            {
                var retVal = instancesAvailable.Pop();
                instancesInUse.Add(retVal);

                return retVal;
            }

            int instancesToCreate = NewInstanceToCreate();
            if (instancesAvailable.Count == 0 && OverflowBehavior != PoolOverflowBehavior.EXCEPTION) { return null; }

            AssertUtil.Greater(instancesToCreate, 0, new PoolException("Invalid Instance length to create", PoolExceptionType.NO_INSTANCE_TO_CREATE));
            AssertUtil.NotNull(InstanceProvider, new PoolException("A Pool of type: " + poolType + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER));

            for (int i = 0; i < instancesToCreate; i++)
            {
                var newInstance = GetNewInstance();
                Add(newInstance);
            }

            return GetInstanceInternal();
        }

        protected object GetNewInstance()
        {
            return InstanceProvider.GetInstance(poolType);
        }


        #endregion


        #region IManagedList Implementation

        public IManagedList Add(object value)
        {
            AssertUtil.IsInstanceOf(poolType, value, new PoolException( "Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + poolType.ToString() + "\n\t\tMismatch type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH));
            _instanceCount++;
            instancesAvailable.Push(value);
            return this;
        }

        public IManagedList Add(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                this.Add(list[i]);
            }

            return this;
        }

        public IManagedList Remove(object value)
        {
            _instanceCount--;
            RemoveInstance(value);
            return this;
        }

        public IManagedList Remove(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                this.Remove(list[i]);
            }

            return this;
        }

        public bool Contains(object o)
        {
            return instancesInUse.Contains(o);
        }


        #endregion



    }


}