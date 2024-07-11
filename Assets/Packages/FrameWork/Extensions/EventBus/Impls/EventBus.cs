using Cr7Sund.Package.Api;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Impl;

namespace Cr7Sund.Package.EventBus.Impl
{
    public class EventBus : GenericEventBus<IEventData>, IEventBus
    {
        [Inject] private IPoolBinder _poolBinder;
        protected override IPoolBinder poolBinder => _poolBinder;


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

        public TEvent CreateEvent<TEvent>() where TEvent : IEventData, new()
        {
            return _poolBinder.AutoCreate<TEvent>();
        }

        public bool DispatchSignal<TEvent>() where TEvent : IEventData, new()
        {
            var @event = _poolBinder.AutoCreate<TEvent>();
            return base.Raise(@event);
        }
    }
}