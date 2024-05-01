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
            return base.Raise<TEvent>(@event);
        }

        public bool DispatchImmediately<TEvent>(TEvent @event) where TEvent : IEventData, new()
        {
            return this.RaiseImmediately<TEvent>(@event);
        }

        public void AddObserver<TEvent>(Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            this.SubscribeTo<TEvent>(handler);
        }

        public void RemoveObserver<TEvent>(Api.EventHandler<TEvent> handler) where TEvent : IEventData, new()
        {
            this.UnsubscribeFrom<TEvent>(handler);
        }

        public TEvent CreateEvent<TEvent>() where TEvent : IEventData, new()
        {
            return _poolBinder.AutoCreate<TEvent>();
        }
    }
}