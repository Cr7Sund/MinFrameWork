namespace Cr7Sund.LifeTime
{
    public interface ILifeCycle
    {
        PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext);
        PromiseTask OnStart(UnsafeCancellationToken cancellation);
        PromiseTask OnEnable(UnsafeCancellationToken cancellation);
        PromiseTask OnDisable(UnsafeCancellationToken cancellation);
        PromiseTask OnStop(UnsafeCancellationToken cancellation);
        void OnDestroy();
    }

}
