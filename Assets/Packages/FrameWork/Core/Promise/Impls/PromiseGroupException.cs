using Cr7Sund.PackageTest.Api;
using System;
namespace Cr7Sund.PackageTest.Impl
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
