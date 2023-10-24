/**
 * @interface Cr7Sund.Framework.Api.IEventDispatcher
 * 
 * Interface for allowing a client to register as an observer.
 * 
 * EventDispatcher allows a client to register as an observer. Whenever the 
 * Dispatcher executes a `Dispatch()`, observers will be notified of any event
 * (Key) for which they have registered.
 * 
 * EventDispatcher dispatches IEvents.
 * 
 * @see Cr7Sund.Framework.Api.IEvent
 */

namespace Cr7Sund.Framework.Api
{
    public interface IEventDispatcher : IDispatcher, ITriggerProvider, ITriggerable
    {
        /// Add an observer with exactly one argument to this Dispatcher
        void AddListener(object evt, EventCallback callback);

        /// Add an observer with exactly no arguments to this Dispatcher
        void AddListener(object evt, EmptyCallback callback);

        /// Remove a previously registered observer with exactly one argument from this Dispatcher
        void RemoveListener(object evt, EventCallback callback);

        /// Remove a previously registered observer with exactly no arguments from this Dispatcher
        void RemoveListener(object evt, EmptyCallback callback);

        /// Returns true if the provided observer is already registered
        bool HasListener(object evt, EventCallback callback);

        /// Returns true if the provided observer is already registered
        bool HasListener(object evt, EmptyCallback callback);

        /// Allow a previously retained event to be returned to its pool
        void ReleaseEvent(IEvent evt);
    }
}