using System;

namespace Cr7Sund.Framework.Util
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception exception)
        {
            AssertUtil.NotNull(exception);
            this.Exception = exception;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }
}