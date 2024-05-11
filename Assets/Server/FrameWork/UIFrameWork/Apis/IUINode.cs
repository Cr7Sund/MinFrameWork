using Cr7Sund.NodeTree.Api;
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
        PromiseTask BeforeExit(bool push, IUINode partnerView);

        // Method called before entering the view
        PromiseTask BeforeEnter(bool push, IUINode partnerView);

        // Method called when exiting the view
        PromiseTask Exit(bool push, IUINode partnerView, bool playAnimation);

        // Method called when entering the view
        PromiseTask Enter(bool push, IUINode partnerView, bool playAnimation);

        // Method called after exiting the view
        PromiseTask AfterExit(bool push, IUINode enterPage);
        // Method called after entering the view
        PromiseTask AfterEnter(bool push, IUINode enterPage);
    }
}
