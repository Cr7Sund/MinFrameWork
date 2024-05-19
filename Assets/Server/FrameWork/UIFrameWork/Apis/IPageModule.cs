using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Server.UI.Api
{
    public interface IPageModule : IUIModule
    {
        PromiseTask PreLoadUI(UIKey key);
        public PromiseTask BackPage();
        public PromiseTask BackPage(IAssetKey popKey);
        public PromiseTask BackPage(int popCount);
        public PromiseTask PushPage(IAssetKey uiKey, bool overwrite = false);

        void TimeOut(int elapsedTime);
    }

}
