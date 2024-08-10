using System;

namespace Cr7Sund.FrameWork.Util
{
    public interface IPoolNode<T>
    {
        ref T NextNode { get; }
        /// <summary>
        /// Object Validation
        /// Before returning an object to the pool, set your public state flag to true
        /// when change the flag to true which means it is not already in use elsewhere.
        /// </summary>
        bool IsRecycled { get; set; }
    }

    //since pool is static
    public struct ReusablePool<T> where T : class, IPoolNode<T>
    {
        public static int MaxPoolSize = int.MaxValue;

        int size;
        T root;

        public int Size => size;



        public bool TryPop(out T result)
        {
            var v = root;
            if (!(v is null))
            {
                ref var nextNode = ref v.NextNode;
                root = nextNode;
                nextNode = null;
                size--;
                result = v;
                result.IsRecycled = false;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public bool TryPush(T item)
        {
            if (item.IsRecycled)
            {
                throw new Exception("item is already recycled");
            }

            if (size < MaxPoolSize)
            {
                item.NextNode = root;
                item.IsRecycled = true;
                root = item;
                size++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
