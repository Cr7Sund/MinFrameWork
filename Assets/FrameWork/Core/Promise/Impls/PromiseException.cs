using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PromiseException : Exception
    {
        public PromiseExceptionType Type { get; private set; }

        public PromiseException(string message, PromiseExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
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