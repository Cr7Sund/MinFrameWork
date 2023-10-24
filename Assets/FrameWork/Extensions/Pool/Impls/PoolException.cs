using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PoolException : Exception
    {
        public PoolExceptionType type { get; set; }

        public PoolException() : base()
        {
        }

        /// Constructs a PoolException with a message and PoolExceptionType
        public PoolException(string message, PoolExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
    }
}

