using Cr7Sund;
using Cr7Sund.UGUI.Apis;
namespace Cr7Sund.UILifeTime
{
    public interface IFragmentTransition
    {
        PromiseTask pop_enter(string partnerPage, UnsafeCancellationToken cancellationToken);
        PromiseTask pop_exit(string partnerPage, UnsafeCancellationToken cancellationToken);
        PromiseTask push_enter(string partnerPage, UnsafeCancellationToken cancellationToken);
        PromiseTask push_exit(string partnerPage, UnsafeCancellationToken cancellationToken);
    }

}
