using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class PromiseGroupException : Exception
    {
        public readonly Exception[] Exceptions;

        public PromiseGroupException(int length)
        {
            Exceptions = new Exception[length];
        }
    }
}
