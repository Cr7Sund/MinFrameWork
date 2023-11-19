using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PoolBinder : Binder, IPoolBinder
    {
        private IInstanceProvider _poolInstanceProvider;

        public PoolBinder()
        {
            _poolInstanceProvider = PoolInstanceProvider.Singleton;
        }

        public PoolBinder(IInstanceProvider poolInstanceProvider)
        {
            _poolInstanceProvider = poolInstanceProvider;
        }

        public IPool<T> Get<T>() where T : class, new()
        {
            var type = typeof(T);
            var binding = GetBinding(type);
            IPool<T> retVal = null;
            if (binding == null)
            {
                binding = GetRawBinding();
                retVal = new Pool<T>();
                retVal.poolType = type;
                retVal.InstanceProvider = _poolInstanceProvider;
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value as IPool<T>;
            }

            return retVal;
        }

        public IPool Get(Type type)
        {
            var binding = GetBinding(type);
            IPool retVal = null;
            if (binding == null)
            {
                binding = GetRawBinding();
                retVal = new Pool();
                retVal.poolType = type;
                retVal.InstanceProvider = _poolInstanceProvider;
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value as IPool;
            }

            return retVal;
        }
    }

    public class PoolInstanceProvider : IInstanceProvider
    {
        internal static readonly PoolInstanceProvider Singleton = new PoolInstanceProvider();
        public T GetInstance<T>()
        {
            UnityEngine.Debug.Log(typeof(T));
            return Activator.CreateInstance<T>();
        }

        public object GetInstance(Type key)
        {
            UnityEngine.Debug.Log(key);
            return Activator.CreateInstance(key);
        }
    }
}