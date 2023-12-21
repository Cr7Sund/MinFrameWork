namespace Cr7Sund.EventBus
{
    public class EventBus : GenericEventBus<IEventData>, IEventBus
    {
        public bool Dispatch<TEvent>(TEvent @event) where TEvent : IEventData, new()
        {
            return this.Dispatch<TEvent>(@event);
        }

        public bool DispatchImmediately<TEvent>(TEvent @event) where TEvent : IEventData, new()
        {
            return this.DispatchImmediately<TEvent>(@event);
        }
    }
}