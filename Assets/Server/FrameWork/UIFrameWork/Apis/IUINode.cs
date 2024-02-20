using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Server.UI.Api
{
    public interface IUINode : INode
    {
        //  get the UI controller associated with the view
        IUIController Controller { get; }

        //  get the UI panel associated with the view
        IUIView View { get; }

        //  get or set the page ID
        string PageId { get; set; }

        //  check if the view is currently transitioning
        bool IsTransitioning { get; }



        // Method called before exiting the view
        IPromise BeforeExit(bool push, IUINode partnerView);

        // Method called before entering the view
        IPromise BeforeEnter(bool push, IUINode partnerView);

        // Method called when exiting the view
        IPromise Exit(bool push, IUINode partnerView, bool playAnimation);

        // Method called when entering the view
        IPromise Enter(bool push, IUINode partnerView, bool playAnimation);

        // Method called after exiting the view
        IPromise AfterExit(bool push, IUINode enterPage);
        // Method called after entering the view
        IPromise AfterEnter(bool push, IUINode enterPage);
    }
}
