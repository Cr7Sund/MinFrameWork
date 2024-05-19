using System;
using System.Diagnostics;
using System.Threading;
using Cr7Sund.CompilerServices;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund
{
    public sealed class PromiseTaskSource : IPromiseTaskSource, IPoolNode<PromiseTaskSource>
    {
        private static ReusablePool<PromiseTaskSource> _pool;

        private PromiseTaskSource _nextNode;
        private PromiseTaskCompletionSourceCore core;
        private short version;

        public ref PromiseTaskSource NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        public PromiseTask Task
        {
            [DebuggerHidden]
            get
            {
                return new PromiseTask(this, core.Version);
            }
        }

        private PromiseTaskSource()
        {

        }

        public static PromiseTaskSource Create()
        {
            if (!_pool.TryPop(out PromiseTaskSource result))
            {
                result = new PromiseTaskSource();
            }
            result.version = result.core.Version;
            // TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }

        public bool TryCancel(string cancelMsg, UnsafeCancellationToken cancellation)
        {
            return version == core.Version && core.TrySetCanceled(cancelMsg, cancellation);
        }

        public bool TryResolve()
        {
            return version == core.Version && core.TrySetResult();
        }

        public bool TryReject(Exception exception)
        {
            return version == core.Version && core.TrySetException(exception);
        }

        private bool TryReturn()
        {
            //TaskTracker.RemoveTracking(this);
            core.Reset();
            return _pool.TryPush(this);
        }
    }
}
