using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class PromiseTimerException : Exception
    {

        public PromiseTimerException(string message, PromiseTimerExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
        public PromiseTimerExceptionType Type { get; private set; }
    }
}
