using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class DispatcherException : Exception
    {
        public DispatcherExceptionType type { get; set; }

        public DispatcherException() : base()
        { }

        /// Constructs a DispatcherException with a message and DispatcherExceptionType
        public DispatcherException(string message, DispatcherExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
    }
}

