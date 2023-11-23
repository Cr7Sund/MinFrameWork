using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class InjectionException : Exception
    {

        /// Constructs a InjectionException with a message and BinderExceptionType
        public InjectionException(string message, InjectionExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
        public InjectionExceptionType Type { get; set; }
    }


}
