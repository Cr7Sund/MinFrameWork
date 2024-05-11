using Cr7Sund.Performance;
using System;
using System.Collections.Generic;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    public class PoolBinder : Binder, IPoolBinder
    {
        private readonly IInstanceProvider _poolInstanceProvider;

        public PoolBinder()
        {
            _poolInstanceProvider = new PoolInstanceProvider();
            MemoryMonitor.Register(CleanUnreference);
        }


        public IPool<T> GetOrCreate<T>() where T : new()
        {
            return GetOrCreate<T>(Pool.Default_POOL_MAX_COUNT);
        }

        public IPool<T> GetOrCreate<T>(int maxPoolCount) where T : new()
        {
            var type = typeof(T);
            var binding = GetBinding(type);
            IPool<T> retVal;
            if (binding == null)
            {
                binding = GetRawBinding();
                retVal = new Pool<T>
                {
                    InstanceProvider = _poolInstanceProvider,
                    MaxCount = maxPoolCount
                };
                binding.Bind(type).To(retVal);
            }
            else
            {
                retVal = binding.Value.SingleValue as IPool<T>;
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
                retVal = binding.Value.SingleValue as IPool;
            }

            return retVal;
        }

        public IPool Get(Type type)
        {
            var binding = GetBinding(type);
            IPool retVal = null;
            if (binding != null)
            {
                retVal = binding.Value.SingleValue as IPool;
            }

            return retVal;
        }

        public IPool<T> Get<T>() where T : new()
        {
            var binding = GetBinding(typeof(T));
            IPool<T> retVal = null;
            if (binding != null)
            {
                retVal = binding.Value.SingleValue as IPool<T>;
            }

            return retVal;
        }


        public override void Dispose()
        {
            base.Dispose();
            MemoryMonitor.UnRegister(CleanUnreference);
        }

        public void CleanUnreference()
        {
            var deleteList = new List<IBinding>();
            foreach (var item in _bindings)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    IBinding binding = item.Value[i];
                    CleanBinding(deleteList, binding);
                }
            }

            foreach (var binding in deleteList)
            {
                Unbind(binding);
            }

            void CleanBinding(List<IBinding> deleteList, IBinding binding)
            {
                if (binding.Value.SingleValue is IBasePool pool)
                {
                    if (!pool.IsRetain)
                    {
                        pool.Release();
                        deleteList.Add(binding);
                    }
                }
            }
        }

        public int Test_GetPoolCount()
        {
            return _bindings.Count;
        }
    }

    public static class PoolBinderExtension
    {
        public static T AutoCreate<T>(this IPoolBinder poolBinder) where T : new()
        {
            return poolBinder.GetOrCreate<T>().GetInstance();
        }

        public static void Return<T>(this IPoolBinder poolBinder, T value) where T : new()
        {
            var pool = poolBinder.Get<T>();
            pool.ReturnInstance(value);
        }
    }
}
