using System;

namespace Cr7Sund
{
    internal sealed class IsCanceledSource : IPromiseTaskSource<bool>
    {
        readonly IPromiseTaskSource source;

        public IsCanceledSource(IPromiseTaskSource source)
        {
            this.source = source;
        }

        public bool GetResult(short token)
        {
            if (source.GetStatus(token) == PromiseTaskStatus.Canceled)
            {
                return true;
            }

            source.GetResult(token);
            return false;
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return source.GetStatus(token);
        }

#if DEBUG
        public PromiseTaskStatus UnsafeGetStatus()
        {
            return source.UnsafeGetStatus();
        }
#endif

        public void OnCompleted(Action continuation, short token)
        {
            source.OnCompleted(continuation, token);
        }
    }
}
