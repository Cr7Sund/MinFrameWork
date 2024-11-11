namespace Cr7Sund.LifeTime
{
    public interface ITransition
    {
        // PromiseTask pop_enter(IFragmentKey targetPage, string partnerPage, UnsafeCancellationToken cancellationToken);
        // PromiseTask pop_exit(IFragmentKey targetPage, string partnerPage, UnsafeCancellationToken cancellationToken);
        // PromiseTask push_enter(IFragmentKey targetPage, string partnerPage, UnsafeCancellationToken cancellationToken);
        // PromiseTask push_exit(IFragmentKey targetPage, string partnerPage, UnsafeCancellationToken cancellationToken);

        PromiseTask Show(IRouteKey targetPage, string partnerPage, bool push);
        PromiseTask Hide(IRouteKey targetPage, string partnerPage, bool push);
    }
}
