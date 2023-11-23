using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class EventDispatcherException : Exception
    {

        public EventDispatcherException()
        {
        }

        /// Constructs an EventDispatcherException with a message and EventDispatcherExceptionType
        public EventDispatcherException(string message, EventDispatcherExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
        public EventDispatcherExceptionType Type { get; set; }
    }

}
