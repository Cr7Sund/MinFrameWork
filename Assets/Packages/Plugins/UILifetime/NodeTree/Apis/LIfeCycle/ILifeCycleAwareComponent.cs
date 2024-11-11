namespace Cr7Sund.LifeTime
{
    public interface ILifeCycleAwareComponent
    {
        PromiseTask OnCreate(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation, IRouteArgs fragmentContext);
        PromiseTask OnStart(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation);
        PromiseTask OnResume(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation);
        PromiseTask OnPause(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation);
        PromiseTask OnStop(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation);
        void OnDestroy(ILifeCycleOwner lifeCycleOwner);
    }
}
