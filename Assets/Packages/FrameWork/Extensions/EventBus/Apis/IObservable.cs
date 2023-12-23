
namespace Cr7Sund.EventBus
{
    public interface IObservable
    {
        void AddObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();

        void RemoveObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();
    }
}