using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.Package.EventBus.Impl
{
    public class EventBus : GenericEventBus<IEventData>, IEventBus
    {
        public bool Dispatch<TEvent>(TEvent @event) where TEvent : IEventData, new()
        {
            return base.Raise(@event);
        }

        public bool DispatchImmediately<TEvent>(TEvent @event) where TEvent : IEventData, new()
        {
            return this.RaiseImmediately(@event);
        }

        public void AddObserver<TEvent>(Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            this.SubscribeTo(handler);
        }

        public void RemoveObserver<TEvent>(Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            this.UnsubscribeFrom(handler);
        }
    }
}