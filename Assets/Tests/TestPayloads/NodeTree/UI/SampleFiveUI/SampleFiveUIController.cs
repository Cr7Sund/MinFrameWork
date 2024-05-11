using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleFiveUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }

        protected override async PromiseTask OnStart(CancellationToken cancellation)
        {
            Debug.Debug("Load ui five");
            await base.OnStart(cancellation);

            StartValue++;
        }

        protected override async PromiseTask OnEnable()
        {
            await base.OnEnable();
            Debug.Debug("Enable ui five");

            EnableCount++;
        }

        protected override async PromiseTask OnDisable()
        {
            Debug.Debug("Disable ui five");
            await base.OnDisable();

            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui five");
            await base.OnStop();
            StartValue--;
        }
    }
}