using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public abstract class BaseLifeCycleAwareComponent : ILifeCycleAwareComponent
    {

        public PromiseTask OnCreate(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation, IRouteArgs fragmentContext)
        {
            return PromiseTask.CompletedTask;
        }
        public PromiseTask OnStart(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation)
        {
            return PromiseTask.CompletedTask;
        }
        public PromiseTask OnResume(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation)
        {
            return PromiseTask.CompletedTask;
        }
        public PromiseTask OnPause(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation)
        {
            return PromiseTask.CompletedTask;
        }
        public PromiseTask OnStop(ILifeCycleOwner lifeCycleOwner, UnsafeCancellationToken cancellation)
        {
            return PromiseTask.CompletedTask;
        }
        public void OnDestroy(ILifeCycleOwner lifeCycleOwner)
        {

        }
    }
}