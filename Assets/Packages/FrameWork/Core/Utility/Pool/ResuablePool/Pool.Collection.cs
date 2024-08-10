using System.Collections.Generic;

namespace Cr7Sund.FrameWork.Util
{
    public class QueuePoolNode<T> : Queue<T>, IPoolNode<QueuePoolNode<T>>
    {
        private QueuePoolNode<T> _poolListNode;


        public ref QueuePoolNode<T> NextNode => ref _poolListNode;
        public bool IsRecycled { get; set; }

        public void TryReturn(ref ReusablePool<QueuePoolNode<T>> reusablePool)
        {

            reusablePool.TryPush(this);
        }

        public static QueuePoolNode<T> Create(ref ReusablePool<QueuePoolNode<T>> reusablePool)
        {
            if (!reusablePool.TryPop(out var resultHandlers))
            {
                resultHandlers = new QueuePoolNode<T>();
            }
            return resultHandlers;
        }
    }

    public class ListPoolNode<T> : List<T>, IPoolNode<ListPoolNode<T>>
    {
        private ListPoolNode<T> _poolListNode;

        public ref ListPoolNode<T> NextNode => ref _poolListNode;
        public bool IsRecycled { get; set; }

        public void TryReturn(ref ReusablePool<ListPoolNode<T>> reusablePool)
        {

            reusablePool.TryPush(this);
        }

        public static ListPoolNode<T> Create(ref ReusablePool<ListPoolNode<T>> reusablePool)
        {
            if (!reusablePool.TryPop(out var resultHandlers))
            {
                resultHandlers = new ListPoolNode<T>();
            }
            return resultHandlers;
        }
    }

}
