namespace Cr7Sund.Server.UI.Impl
{
    public static class UICreator
    {
        public static UINode Create(UIKey key)
        {
            return new UINode(key, key.CreateView(), key.CreateCtrl());
        }
    }
}
