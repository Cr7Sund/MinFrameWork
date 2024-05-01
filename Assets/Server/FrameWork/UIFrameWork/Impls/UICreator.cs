namespace Cr7Sund.Server.UI.Impl
{
    public static class UICreator
    {
        public static PageNode CreatePageNode(UIKey key)
        {
            return new PageNode(key, key.CreateView(), key.CreateCtrl());
        }

        public static PanelNode CreatePanelNode(UIKey key)
        {
            return new PanelNode(key, key.CreateView(), key.CreateCtrl());
        }

    }
}
