using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
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
