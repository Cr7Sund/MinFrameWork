/**
 * @class Cr7Sund.Framework.Impl.ReflectionException
 *
 * An exception thrown by the Reflector.
 */

using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class ReflectionException : Exception
    {

        public ReflectionException()
        {
        }

        /// Constructs a ReflectionException with a message and ReflectionExceptionType
        public ReflectionException(string message, ReflectionExceptionType exceptionType) : base(message)
        {
            type = exceptionType;
        }
        public ReflectionExceptionType type { get; set; }
    }
}
