
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PageNode : UINode
    {
        [Inject] IPanelModule _panelModule;


        public PageNode(IAssetKey assetKey, IUIView uiView, IUIController uiController) : base(assetKey, uiView, uiController)
        {

        }

        protected override async PromiseTask OnUnloadAsync()
        {
            await _panelModule.CloseAll();
            await base.OnUnloadAsync();
        }
    }
}