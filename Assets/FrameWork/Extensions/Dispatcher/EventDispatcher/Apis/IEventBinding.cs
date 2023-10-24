/**
 * @interface Cr7Sund.Framework.Api.IEventBinding
 * 
 * Binding interface for EventDispatcher.
 * 
 * EventBindings technically allow any Key, but require either an 
 * EmptyCallback (no arguments) or an EventCallback (one argument).
 * 
 * The IEvent only accepts strings as keys, so in the standard MVCSContext
 * setup, your EventBinder keys should also be strings.
 * 
 * @see Cr7Sund.Framework.Api.IEvent
 */

using System;

namespace Cr7Sund.Framework.Api
{
    public interface IEventBinding : IBinding
    {
        /// Retrieve the type of the provided callback
        EventCallbackType TypeForCallback(EventCallback callback);

        /// Retrieve the type of the provided callback
        EventCallbackType TypeForCallback(EmptyCallback callback);

        new IEventBinding Bind(object key);
        IEventBinding ToValue(EventCallback callback);
        IEventBinding ToValue(EmptyCallback callback);
    }

    /// Delegate for adding a listener with a single argument
    public delegate void EventCallback(IEvent payload);

    /// Delegate for adding a listener with a no arguments
    public delegate void EmptyCallback();

    public enum EventCallbackType
    {
        /// Indicates an EventCallback with no arguments
        NO_ARGUMENTS,
        /// Indicates an EventCallback with one argument
        ONE_ARGUMENT,
        /// Indicates no matching EventCallback could be found
        NOT_FOUND
    }
}