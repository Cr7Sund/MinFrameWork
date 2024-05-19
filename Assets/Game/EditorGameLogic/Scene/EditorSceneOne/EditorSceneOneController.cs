using System.Threading;
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

        protected async override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            await base.OnStart(cancellation);
            Debug.Debug("Load scene one");
        }

        protected async override PromiseTask OnEnable()
        {
            Debug.Debug("Enable scene one");

            try
            {
                await _pageContainer.PushPage(UI.EditorUIKeys.SampleOneUI);
            }
            catch (System.Exception ex)
            {
                Debug.Info(ex);
            }
        }

        protected async override PromiseTask OnDisable(bool closeImmediately)
        {
            await base.OnDisable(closeImmediately);
            Debug.Debug("Disable scene one");
        }

        protected async override PromiseTask OnStop()
        {
            await base.OnStop();
            Debug.Debug("Stop scene one");
        }
    }
}
