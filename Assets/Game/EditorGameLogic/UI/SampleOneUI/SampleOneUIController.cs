using System.Threading;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleOneUIController : BaseUIController
    {
        public int StartValue;
        public int EnableCount;

        [Inject] private IPageModule _pageContainer;


        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load ui one {StartValue}", StartValue);
            await base.OnStart(cancellation);
            StartValue++;
        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable ui one");

            try
            {
                // loaded
                await _pageContainer.PushPage(EditorUIKeys.SampleTwoUI, true);
            }
            catch (System.Exception ex)
            {
                Debug.Info(ex);
            }
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            Debug.Debug("Disable ui one");
            await base.OnDisable(closeImmediately);
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
