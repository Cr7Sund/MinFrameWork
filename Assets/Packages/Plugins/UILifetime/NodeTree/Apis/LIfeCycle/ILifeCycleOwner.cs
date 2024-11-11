namespace Cr7Sund.LifeTime
{
    public interface ILifeCycleOwner
    {
        LifeCycleState lifeCycleState { get; }

        PromiseTask MarkState(LifeCycleState targetState, UnsafeCancellationToken cancellation, IRouteArgs fragmentContext = null);
        PromiseTask AddLifecycle(ILifeCycleAwareComponent lifeCycle, IRouteArgs fragmentContext);
        PromiseTask RemoveLifecycle(ILifeCycleAwareComponent lifeCycle);
    }
}
