using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
	public class EventDispatcherException : Exception
	{
		public EventDispatcherExceptionType type{ get; set;}

		public EventDispatcherException() : base()
		{
		}

		/// Constructs an EventDispatcherException with a message and EventDispatcherExceptionType
		public EventDispatcherException(string message, EventDispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}

}