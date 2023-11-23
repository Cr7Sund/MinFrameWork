using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class BinderException : Exception
    {

        /// Constructs a BinderException with a message and BinderExceptionType
        public BinderException(string message, BinderExceptionType exceptionType) : base(message)
        {
            Type = exceptionType;
        }
        public BinderExceptionType Type { get; set; }
    }


}
