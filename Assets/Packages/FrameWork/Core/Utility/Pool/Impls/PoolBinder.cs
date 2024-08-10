using System;
using System.Collections.Generic;

namespace Cr7Sund.FrameWork.Util
{
    public class PoolBinder : IPoolBinder
    {
        private Dictionary<Type, IBasePool> _containers;
        private readonly IInstanceProvider _poolInstanceProvider;

        public int Count => _containers.Count;



        public PoolBinder()
        {
            _containers = new();
            _poolInstanceProvider = new PoolInstanceProvider();
        }


        public IPool<T> GetOrCreate<T>() where T : new()
        {
            return GetOrCreate<T>(global::Cr7Sund.FrameWork.Util.Pool.Default_POOL_MAX_COUNT);
        }

        public IPool<T> GetOrCreate<T>(int maxPoolCount) where T : new()
        {
            var type = typeof(T);
            if (!_containers.TryGetValue(type, out var retVal))
            {
                retVal = new Pool<T>
                {
                    InstanceProvider = _poolInstanceProvider,
                    MaxCount = maxPoolCount
                };
                _containers[type] = retVal;
            }

            return retVal as IPool<T>;
        }

        public IPool GetOrCreate(Type type)
        {
            if (!_containers.TryGetValue(type, out var retVal))
            {
                retVal = new global::Cr7Sund.FrameWork.Util.Pool
                {
                    InstanceProvider = _poolInstanceProvider,
                    PoolType = type
                };
                _containers[type] = retVal;
            }

            return retVal as IPool;
        }

        public IPool Get(Type type)
        {
            if (_containers.TryGetValue(type, out var retVal))
            {
                return retVal as IPool;
            }
            else
            {
                return default;
            }
        }

        public IPool<T> Get<T>() where T : new()
        {
            if (_containers.TryGetValue(typeof(T), out var retVal))
            {
                return retVal as IPool<T>;
            }
            else
            {
                return default;
            }
        }


        public void Dispose()
        {
        }

        public void CleanUnreference()
        {
            var deleteList = new List<Type>();
            foreach (var item in _containers)
            {
                var pool = item.Value;
                if (!pool.IsRetain)
                {
                    pool.Release();
                    deleteList.Add(item.Key);
                }
            }

            foreach (var key in deleteList)
            {
                _containers.Remove(key);
            }

        }

        public int Test_GetPoolCount()
        {
            return _containers.Count;
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

        public static void Return<T, TValue>(this IPoolBinder poolBinder, T value) where T : ICollection<TValue>, new()
        {
            value.Clear();
            var pool = poolBinder.Get<T>();
            pool.ReturnInstance(value);
        }
    }
}
