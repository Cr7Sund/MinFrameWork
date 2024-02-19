using Cr7Sund.Framework.Api;  
using Cr7Sund.UGUI.Apis;     

/**
 * @class Cr7Sund.Framework.Api.IView
 *
 * Monobehaviours must implement this interface in order to be injectable.
 *
 * To contact the Context, the View must be able to find it. View handles this
 * with bubbling.
 */
namespace Cr7Sund.Server.UI.Api
{
    public interface IUIView
    {
        //  get the UI controller associated with the view
        IUIController Controller { get; }

        //  get the UI panel associated with the view
        IUIPanel View { get; }

        //  get or set the page ID
        string PageId { get; set; }

        //  check if the view is currently transitioning
        bool IsTransitioning { get; }

        //  get the asset key associated with the view
        IAssetKey Key { get; }

        // Method called before exiting the view
        IPromise BeforeExit(bool push, IUIView partnerView);

        // Method called before entering the view
        IPromise BeforeEnter(bool push, IUIView partnerView);

        // Method called when exiting the view
        IPromise Exit(bool push, IUIView partnerView, bool playAnimation);

        // Method called when entering the view
        IPromise Enter(bool push, IUIView partnerView, bool playAnimation);

        // Method called after exiting the view
        IPromise AfterExit(bool push, IUIView enterPage);

        // Method called after entering the view
        IPromise AfterEnter(bool push, IUIView enterPage);
    }
}
