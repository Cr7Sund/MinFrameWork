using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class InjectionException : Exception
    {
        public InjectionExceptionType Type { get; set; }

        /// Constructs a InjectionException with a message and BinderExceptionType
        public InjectionException(string message, InjectionExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
    }


}