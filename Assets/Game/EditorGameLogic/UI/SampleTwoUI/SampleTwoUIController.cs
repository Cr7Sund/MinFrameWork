using System.Threading;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleTwoUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;


        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load ui two");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable ui two");
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            Debug.Debug("Disable ui two");
            await base.OnDisable(closeImmediately);
            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui two");
            await base.OnStop();
            StartValue--;
        }
    }
}