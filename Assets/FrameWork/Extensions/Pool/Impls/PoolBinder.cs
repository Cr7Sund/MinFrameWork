using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PoolBinder : Binder, IPoolBinder
    {
        private static IInstanceProvider poolInstanceProvider;
        public PoolBinder()
        {
            if (poolInstanceProvider == null)
            {
                poolInstanceProvider = new PoolInstanceProvider();
            }
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
                retVal.InstanceProvider = poolInstanceProvider;
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value as IPool;
            }

            return retVal;
        }
    }

    class PoolInstanceProvider : IInstanceProvider
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