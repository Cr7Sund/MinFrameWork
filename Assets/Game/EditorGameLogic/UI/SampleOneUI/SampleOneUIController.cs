using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleOneUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        [Inject] private IPageModule _pageContainer;

        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }

        protected override async PromiseTask OnStart()
        {
            Debug.Debug("Load ui one {StartValue}", StartValue);
            await base.OnStart();
            StartValue++;
        }

        protected override async PromiseTask OnEnable()
        {
            await _pageContainer.PushPage(EditorUIKeys.SampleTwoUI);
            Debug.Debug("Enable ui one");
            EnableCount++;
        }

        protected override async PromiseTask OnDisable()
        {
            Debug.Debug("Disable ui one");
            await base.OnDisable();
            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui one");
            await base.OnStop();
            StartValue--;
        }
    }
}
