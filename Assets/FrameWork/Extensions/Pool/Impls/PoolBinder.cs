using Cr7Sund.Framework.Api;
using System;
using UnityEngine;
namespace Cr7Sund.Framework.Impl
{
    public class PoolBinder : Binder, IPoolBinder
    {
        private readonly IInstanceProvider _poolInstanceProvider;

        public PoolBinder()
        {
            _poolInstanceProvider = new PoolInstanceProvider();
        }

        public PoolBinder(IInstanceProvider poolInstanceProvider)
        {
            _poolInstanceProvider = poolInstanceProvider;
        }

        public IPool<T> GetOrCreate<T>() where T : class, new()
        {
            var type = typeof(T);
            var binding = GetBinding(type);
            IPool<T> retVal = null;
            if (binding == null)
            {
                binding = GetRawBinding();
                retVal = new Pool<T>();
                retVal.InstanceProvider = _poolInstanceProvider;
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value as IPool<T>;
            }

            return retVal;
        }

        public IPool GetOrCreate(Type type)
        {
            var binding = GetBinding(type);
            IPool retVal = null;
            if (binding == null)
            {
                binding = GetRawBinding();
                retVal = new Pool();
                retVal.PoolType = type;
                retVal.InstanceProvider = _poolInstanceProvider;
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value as IPool;
            }

            return retVal;
        }

        public IPool Get(Type type)
        {
            var binding = GetBinding(type);
            IPool retVal = null;
            if (binding != null)
            {
                retVal = binding.Value as IPool;
            }

            return retVal;
        }

        public IPool<T> Get<T>() where T : class, new()
        {
            var binding = GetBinding(typeof(T));
            IPool<T> retVal = null;
            if (binding != null)
            {
                retVal = binding.Value as IPool<T>;
            }

            return retVal;
        }
    }

    public class PoolInstanceProvider : IInstanceProvider
    {
        public T GetInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public object GetInstance(Type key)
        {
            return Activator.CreateInstance(key);
        }
    }
}
