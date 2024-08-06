namespace Cr7Sund.Server.UI.Api
{
    public interface IPanelModule : IUIModule
    {
        public PromiseTask OpenPanel(IAssetKey uiKey);
        public PromiseTask OpenPanelAndCloseOthers(IAssetKey uiKey);
        public void TimeOut(long milliseconds);
    }

}
