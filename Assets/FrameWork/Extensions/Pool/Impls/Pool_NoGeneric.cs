using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Impl
{

    public class Pool : BasePool, IPool
    {

        /// Stack of instances still in the Pool.
        protected Stack _instancesAvailable;


        public Pool() : base()
        {
            InstancesInUse = new HashSet<object>();
            _instancesAvailable = new Stack();
        }
        private HashSet<object> InstancesInUse
        {
            get;
        }
        public override int Available
        {
            get
            {
                return _instancesAvailable.Count;
            }
        }
        public object Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public Type PoolType { get; set; }

        public override void Clean()
        {
            _instancesAvailable.Clear();
            InstancesInUse.Clear();
            base.Clean();
        }

        #region IPool Implementation
        public object GetInstance()
        {
            object instance = GetInstanceInternal();
            if (instance is IPoolable)
            {
                (instance as IPoolable).Retain();
            }
            return instance;
        }

        public void ReturnInstance(object value)
        {
            if (InstancesInUse.Contains(value))
            {
                if (value is IPoolable)
                {
                    (value as IPoolable).Restore();
                }
                InstancesInUse.Remove(value);
                _instancesAvailable.Push(value);
            }
        }

        private void RemoveInstance(object value)
        {
            AssertUtil.IsInstanceOf(PoolType, value, PoolExceptionType.TYPE_MISMATCH);


            if (InstancesInUse.Contains(value))
            {
                InstancesInUse.Remove(value);
            }
            else
            {
                _instancesAvailable.Pop();
            }
        }

        private object GetInstanceInternal()
        {
            if (_instancesAvailable.Count > 0)
            {
                var retVal = _instancesAvailable.Pop();
                InstancesInUse.Add(retVal);

                return retVal;
            }
            else
            {
                CreateInstancesIfNeeded();
                if (_instancesAvailable.Count == 0 && OverflowBehavior != PoolOverflowBehavior.EXCEPTION)
                {
                    return null;
                }

                var retVal = _instancesAvailable.Pop();
                InstancesInUse.Add(retVal);
                return retVal;
            }
        }

        private void CreateInstancesIfNeeded()
        {
            int instancesToCreate = NewInstanceToCreate();

            if (instancesToCreate == 0 && OverflowBehavior != PoolOverflowBehavior.EXCEPTION) return;
            AssertUtil.Greater(instancesToCreate, 0, PoolExceptionType.NO_INSTANCE_TO_CREATE);
            if (InstanceProvider == null)
            {
                throw new MyException("A Pool of type: " + PoolType + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER);
            }

            for (int i = 0; i < instancesToCreate; i++)
            {
                var newInstance = GetNewInstance();
                Add(newInstance);
            }
        }


        protected object GetNewInstance()
        {
            return InstanceProvider.GetInstance(PoolType);
        }
        #endregion


        #region IManagedList Implementation
        public IManagedList Add(object value)
        {
            AssertUtil.IsInstanceOf(PoolType, value, PoolExceptionType.TYPE_MISMATCH);
            IncreaseInstance();
            _instancesAvailable.Push(value);
            return this;
        }


        public IManagedList Add(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Add(list[i]);
            }

            return this;
        }

        public IManagedList Remove(object value)
        {
            DecreaseInstance();
            RemoveInstance(value);
            return this;
        }



        public IManagedList Remove(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Remove(list[i]);
            }

            return this;
        }

        public void Dispose()
        {
            InstancesInUse.Clear();
            _instancesAvailable.Clear();
            ClearInstances();
        }

        public bool Contains(object o)
        {
            return InstancesInUse.Contains(o);
        }
        #endregion
    }


}
