using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class DispatcherException : Exception
    {

        /// Constructs a DispatcherException with a message and DispatcherExceptionType
        public DispatcherException(string message, DispatcherExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
        public DispatcherExceptionType type { get; set; }
    }
}
