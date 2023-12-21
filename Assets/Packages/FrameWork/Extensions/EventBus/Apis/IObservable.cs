using System;

namespace Cr7Sund.EventBus
{
    public interface IObservable
    {
        bool AddObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();

        bool RemoveObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();
    }
}