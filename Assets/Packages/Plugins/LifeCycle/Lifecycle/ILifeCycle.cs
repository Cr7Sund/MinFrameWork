
namespace Cr7Sund.LifeCycle
{
    public interface ILifeCycle
    {
        PromiseTask OnCreateAsync(ILifeCycleOwner owner,UnsafeCancellationToken cancellationToken);
        void OnStart(ILifeCycleOwner owner);
        void OnResume(ILifeCycleOwner owner);
        void OnPause(ILifeCycleOwner owner);
        void OnStop(ILifeCycleOwner owner);
        void OnDestroy(ILifeCycleOwner owner);
    }
}
