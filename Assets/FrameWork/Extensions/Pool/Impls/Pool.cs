﻿using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Impl
{
    public class Pool<T> : BasePool, IPool<T> where T : class, new()
    {

        /// Stack of instances still in the Pool.
        private readonly Stack<T> _instancesAvailable;

        public Pool() : base()
        {
            InstancesInUse = new HashSet<T>();
            _instancesAvailable = new Stack<T>();
        }
        private HashSet<T> InstancesInUse
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

        public override void Clean()
        {
            _instancesAvailable.Clear();
            InstancesInUse.Clear();
            base.Clean();
        }

        #region IPool Implementation
        public T GetInstance()
        {
            var instance = GetInstanceInternal();
            if (instance is IPoolable poolable)
            {
                poolable.Retain();
            }
            return instance;
        }

        public void ReturnInstance(T value)
        {
            if (!InstancesInUse.Contains(value)) return;

            if (value is IPoolable poolable)
            {
                poolable.Restore();
            }
            InstancesInUse.Remove(value);
            _instancesAvailable.Push(value);
        }

        public void ReturnInstance(object value)
        {
            AssertUtil.IsInstanceOf<T>(value);

            ReturnInstance((T)value);
        }

        private void RemoveInstance(T value)
        {
            if (InstancesInUse.Contains(value))
            {
                InstancesInUse.Remove(value);
            }
            else
            {
                _instancesAvailable.Pop();
            }
        }

        private T GetInstanceInternal()
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
                throw new MyException("A Pool of type: " + typeof(T) + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER);
            }
            for (int i = 0; i < instancesToCreate; i++)
            {
                var newInstance = GetNewInstance();
                Add(newInstance);
            }
        }

        private T GetNewInstance()
        {
            return InstanceProvider.GetInstance<T>();
        }
        #endregion



        #region IManagedList Implementation
        public IManagedList Add(object value)
        {
            AssertUtil.IsInstanceOf(typeof(T), value, PoolExceptionType.TYPE_MISMATCH);

            IncreaseInstance();
            _instancesAvailable.Push((T)value);
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
            AssertUtil.IsInstanceOf(typeof(T), value, PoolExceptionType.TYPE_MISMATCH);

            DecreaseInstance();
            RemoveInstance((T)value);
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
            foreach (var item in InstancesInUse)
            {
                if (item is IPoolable poolable)
                {
                    poolable.Restore();
                }
            }

            foreach (var item in _instancesAvailable)
            {
                if (item is IPoolable poolable)
                {
                    poolable.Restore();
                }
            }

            InstancesInUse.Clear();
            _instancesAvailable.Clear();
            ClearInstances();
        }

        public bool Contains(object value)
        {
            AssertUtil.IsInstanceOf(typeof(T), value, PoolExceptionType.TYPE_MISMATCH);

            return InstancesInUse.Contains((T)value);
        }
        #endregion
    }
}
