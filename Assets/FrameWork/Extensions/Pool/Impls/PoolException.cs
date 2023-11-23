using System;
namespace Cr7Sund.Framework.Impl
{
    public class PoolException : Exception
    {

        /// Constructs a PoolException with a message and PoolExceptionType
        public PoolException(string message, Enum exceptionType) : base(message)
        {
            type = exceptionType;
        }
        public Enum type { get; set; }
    }
}
