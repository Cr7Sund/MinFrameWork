using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class PromiseException : Exception
    {

        public PromiseException(string message, PromiseExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
        public PromiseExceptionType Type { get; private set; }
    }

    public class PromiseGroupException : Exception
    {
        public readonly Exception[] Exceptions;

        public PromiseGroupException(int length)
        {
            Exceptions = new Exception[length];
        }
    }
}
