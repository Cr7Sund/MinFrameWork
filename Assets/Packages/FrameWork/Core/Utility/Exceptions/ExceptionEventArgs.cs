using System;
namespace Cr7Sund.FrameWork.Util
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception exception)
        {
            AssertUtil.NotNull(exception);
            Exception = exception;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }
}
