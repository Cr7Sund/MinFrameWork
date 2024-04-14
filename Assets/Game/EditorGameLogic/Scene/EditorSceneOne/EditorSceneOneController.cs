using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Game.Scene
{
    public class EditorSceneOneController : BaseSceneController
    {
        [Inject] private IPageModule _pageContainer;
   

        public static void Init()
        {
        }

        protected override async PromiseTask OnStart()
        {
            await base.OnStart();
            Debug.Debug("Load scene one");

        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable scene one ");
            await _pageContainer.PushPage(UI.SampleUIKeys.SampleOneUI);
        }

        protected override async PromiseTask OnDisable()
        {
            await base.OnDisable();
            Debug.Debug("Disable scene one");
        }

        protected override async PromiseTask OnStop()
        {
            await base.OnStop();
            Debug.Debug("Stop scene one");
        }
    }
}
