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

            int instancesToCreate = NewInstanceToCreate();
            if (_instancesAvailable.Count == 0 && OverflowBehavior != PoolOverflowBehavior.EXCEPTION) { return null; }

            AssertUtil.Greater(instancesToCreate, 0, new PoolException("Invalid Instance length to create", PoolExceptionType.NO_INSTANCE_TO_CREATE));
            AssertUtil.NotNull(InstanceProvider, new PoolException("A Pool of type: " + typeof(T) + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER));

            for (int i = 0; i < instancesToCreate; i++)
            {
                var newInstance = GetNewInstance();
                Add(newInstance);
            }

            return GetInstanceInternal();
        }

        private T GetNewInstance()
        {
            return InstanceProvider.GetInstance<T>();
        }
        #endregion



        #region IManagedList Implementation
        public IManagedList Add(object value)
        {
            AssertUtil.IsInstanceOf(typeof(T), value, new PoolException("Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T) + "\n\t\tMismatch type: " + value.GetType(), PoolExceptionType.TYPE_MISMATCH));

            _instanceCount++;
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
            AssertUtil.IsInstanceOf(typeof(T), value, new PoolException("Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T) + "\n\t\tMismatch type: " + value.GetType(), PoolExceptionType.TYPE_MISMATCH));

            _instanceCount--;
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

        public IManagedList Clear()
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
            _instanceCount = 0;
            return this;
        }

        public bool Contains(object value)
        {
            AssertUtil.IsInstanceOf(typeof(T), value, new PoolException("Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T) + "\n\t\tMismatch type: " + value.GetType(), PoolExceptionType.TYPE_MISMATCH));

            return InstancesInUse.Contains((T)value);
        }
        #endregion
    }
}