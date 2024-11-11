using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleFourUIController : BaseUIController
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
            Debug.Debug("Load ui four");
            await base.OnStart(cancellation);

            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            await base.OnEnable();
            Debug.Debug("Enable ui four");
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            Debug.Debug("Disable ui four");
            await base.OnDisable(closeImmediately);

            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui four");
            await base.OnStop();
            StartValue--;
        }
    }
}