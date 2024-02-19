
namespace Cr7Sund.PackageTest.EventBus.Api
{
    public interface IObservable
    {
        void AddObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();

        void RemoveObserver<TEvent>(EventHandler<TEvent> handler) where TEvent : IEventData, new();
    }
}