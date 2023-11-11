using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PromiseTimerException : Exception
    {
        public PromiseTimerExceptionType Type { get; private set; }

        public PromiseTimerException(string message, PromiseTimerExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
    }
}