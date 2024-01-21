namespace Cr7Sund.Server.UI.Api
{
    public struct ViewContent
    {
        public IAssetKey uiKey;

        public object intent;
        public bool playAnimation;
        public bool stack;
        public bool loadAsync;

        public static ViewContent AssignViewContent(IAssetKey uiKey, object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true)
        {
            var viewContent = new ViewContent();
            viewContent.uiKey = uiKey;
            viewContent.intent = intent;
            viewContent.playAnimation = playAnimation;
            viewContent.stack = stack;
            viewContent.loadAsync = loadAsync;

            return viewContent;
        }
    }
}