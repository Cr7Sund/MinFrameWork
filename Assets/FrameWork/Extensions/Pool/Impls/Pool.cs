using System;
using System.Collections;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class Pool<T> : Pool, IPool<T>
    {
        public Pool() : base()
        {
            poolType = typeof(T);
        }

        new public T GetInstance()
        {
            return (T)base.GetInstance();
        }
    }

    public class Pool : IPool, IPoolable
    {
        /// Stack of instances still in the Pool.
        protected Stack instancesAvailable;
        /// A HashSet of the objects checked out of the Pool.
        protected HashSet<object> instancesInUse;
        protected int _instanceCount;
        protected int _size;


        public Pool()
        {
            instancesInUse = new HashSet<object>();
            instancesAvailable = new Stack();

            _size = 0;

            OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
            inflationType = PoolInflationType.DOUBLE;
        }

        #region IPool Implementation
        public IInstanceProvider InstanceProvider { get; set; }

        /// The object Type of the first object added to the pool.
        /// Pool objects must be of the same concrete type. This property enforces that requirement. 
        public Type poolType { get; set; }

        public int Available => instancesAvailable.Count;


        public int instanceCount => _instanceCount;

        public PoolOverflowBehavior OverflowBehavior { get; set; }
        public PoolInflationType inflationType { get; set; }


        public int Count => _size;

        public void SetSize(int size)
        {
            this._size = size;
        }

        public object GetInstance()
        {
            if (instancesAvailable.Count > 0)
            {
                var retVal = instancesAvailable.Pop();
                instancesInUse.Add(retVal);
                return retVal;
            }

            int instancesToCreate = 0;

            // New fixed-size pool. Populate
            if (Count > 0)
            {
                if (instanceCount == 0)
                {
                    instancesToCreate = Count;
                }
                else
                {
                    //Illegal overflow. Report and return null
                    failIf(OverflowBehavior == PoolOverflowBehavior.EXCEPTION,
                        "A pool has overflowed its limit.\n\t\tPool type: " + poolType,
                        PoolExceptionType.OVERFLOW);

                    // Support debug warn
                    if (OverflowBehavior == PoolOverflowBehavior.WARNING)
                    {

                    }
                    return null;
                }
            }
            else
            {
                if (instanceCount == 0 || inflationType == PoolInflationType.INCREMENT)
                {
                    // 1 or 4 
                    instancesToCreate = 1;
                }
                else
                {
                    instancesToCreate = instanceCount;
                }
            }

            if (instancesToCreate > 0)
            {
                failIf(InstanceProvider == null, "A Pool of type: " + poolType + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER);

                for (int i = 0; i < instancesToCreate; i++)
                {
                    var newInstance = InstanceProvider.GetInstance(poolType);
                    Add(newInstance);
                }

                return GetInstance(); // currently have free space
            }

            return null;
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

        public void Clean()
        {
            instancesAvailable.Clear();
            instancesInUse.Clear();
            _instanceCount = 0;
        }

        #endregion

        #region IManagedList Implementation

        public object Value => GetInstance();


        public IManagedList Add(object value)
        {
            failIf(value.GetType() != poolType, "Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + poolType.ToString() + "\n\t\tMismatch type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH);
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

        /// <summary>
        /// Permanently removes an instance from the Pool
        /// </summary>
        /// In the event that the removed Instance is in use, it is removed from instancesInUse.
        /// Otherwise, it is presumed inactive, and the next available object is popped from
        /// instancesAvailable.
        /// <param name="value">An instance to remove permanently from the Pool.</param>
        protected void RemoveInstance(object value)
        {
            failIf(value.GetType() != poolType, "Attempt to remove a instance from a pool that is of the wrong Type:\n\t\tPool type: " + poolType.ToString() + "\n\t\tInstance type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH);
            if (instancesInUse.Contains(value))
            {
                instancesInUse.Remove(value);
            }
            else
            {
                instancesAvailable.Pop();
            }
        }

        protected void failIf(bool condition, string message, PoolExceptionType type)
        {
            if (condition)
            {
                throw new PoolException(message, type);
            }
        }
    }
}