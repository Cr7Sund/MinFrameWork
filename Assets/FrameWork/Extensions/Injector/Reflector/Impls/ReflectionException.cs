/**
 * @class Cr7Sund.Framework.Impl.ReflectionException
 * 
 * An exception thrown by the Reflector.
 */

using System;
using Cr7Sund.Framework.Api;
namespace Cr7Sund.Framework.Impl
{
    public class ReflectionException : Exception
    {
        public ReflectionExceptionType type { get; set; }

        public ReflectionException() : base()
        { }

        /// Constructs a ReflectionException with a message and ReflectionExceptionType
        public ReflectionException(string message, ReflectionExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
    }
}
