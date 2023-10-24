using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class BinderException : Exception
    {
        public BinderExceptionType type { get; set; }

        public BinderException() : base()
        { }

        /// Constructs a BinderException with a message and BinderExceptionType
        public BinderException(string message, BinderExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
    }


}