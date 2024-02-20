namespace Cr7Sund.Package.EventBus.Api
{
    /// <summary>
    /// A delegate for the callback methods given when subscribing to an event type.
    /// </summary>
    /// <param name="eventData">The event that was raised.</param>
    /// <typeparam name="TEvent">The type of event this callback handles.</typeparam>
    public delegate void EventHandler<TEvent>(TEvent eventData) where TEvent : IEventData;

    public interface IEventBus : IObservable
    {
        bool DispatchImmediately<TEvent>(TEvent tEvent) where TEvent : IEventData, new();
        bool Dispatch<TEvent>(TEvent @event) where TEvent : IEventData, new();
    }
}