using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
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
