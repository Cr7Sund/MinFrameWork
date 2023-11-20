using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Impl
{
    public class Pool<T> : BasePool, IPool<T> where T : class, new()
    {
        private HashSet<T> instancesInUse { get; set; }
        public override int Available => instancesAvailable.Count;
        public object Value => throw new System.NotImplementedException();

        /// Stack of instances still in the Pool.
        protected Stack<T> instancesAvailable;

        public Pool()
        {
            instancesInUse = new HashSet<T>();
            instancesAvailable = new Stack<T>();
        }

        public override void Clean()
        {
            instancesAvailable.Clear();
            instancesInUse.Clear();
            base.Clean();
        }

        #region  IPool Implementation

        public T GetInstance()
        {
            var instance = GetInstanceInternal();
            if (instance is IPoolable)
            {
                (instance as IPoolable).Retain();
            }
            return instance;
        }

        public void ReturnInstance(T value)
        {
            if (!instancesInUse.Contains(value)) return;

            if (value is IPoolable)
            {
                (value as IPoolable).Restore();
            }
            instancesInUse.Remove(value);
            instancesAvailable.Push(value);
        }

        public void ReturnInstance(object value)
        {
            AssertUtil.IsInstanceOf<T>(value);

            ReturnInstance((T)value);
        }

        private void RemoveInstance(T value)
        {
            if (instancesInUse.Contains(value))
            {
                instancesInUse.Remove(value);
            }
            else
            {
                instancesAvailable.Pop();
            }
        }

        private T GetInstanceInternal()
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
            AssertUtil.NotNull(InstanceProvider, new PoolException("A Pool of type: " + typeof(T) + " has no instance provider.", PoolExceptionType.NO_INSTANCE_PROVIDER));

            for (int i = 0; i < instancesToCreate; i++)
            {
                var newInstance = GetNewInstance();
                Add(newInstance);
            }

            return GetInstanceInternal();
        }

        protected T GetNewInstance()
        {
            return InstanceProvider.GetInstance<T>();
        }


        #endregion



        #region IManagedList Implementation


        public IManagedList Add(object value)
        {
            AssertUtil.IsInstanceOf( typeof(T), value, new PoolException( "Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T).ToString() + "\n\t\tMismatch type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH));
            
            _instanceCount++;
            instancesAvailable.Push((T)value);
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
            AssertUtil.IsInstanceOf(typeof(T), value, new PoolException("Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T).ToString() + "\n\t\tMismatch type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH));

            _instanceCount--;
            RemoveInstance((T)value);
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

        public bool Contains(object value)
        {
            AssertUtil.IsInstanceOf(typeof(T), value, new PoolException("Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + typeof(T).ToString() + "\n\t\tMismatch type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH));

            return instancesInUse.Contains((T)value);
        }


        #endregion





    }
}
