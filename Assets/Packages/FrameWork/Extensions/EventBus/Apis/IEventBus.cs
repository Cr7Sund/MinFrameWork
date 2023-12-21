using System;

namespace Cr7Sund.EventBus
{
    public interface IEventBus
    {
        bool DispatchImmediately<TEvent>(TEvent tEvent) where TEvent : IEventData, new();
        bool Dispatch<TEvent>(TEvent @event) where TEvent : IEventData, new();
    }
}