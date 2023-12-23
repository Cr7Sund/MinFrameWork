using System;

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

        public void AddObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
             this.SubscribeTo<TEvent>(handler);
        }

        public void RemoveObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            this.UnsubscribeFrom<TEvent>(handler);
        }
    }
}