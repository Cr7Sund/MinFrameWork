using System;
using NUnit.Framework;

namespace Cr7Sund.Framework.Util
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception exception)
        {
            Assert.NotNull(exception);
            this.Exception = exception;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }
}