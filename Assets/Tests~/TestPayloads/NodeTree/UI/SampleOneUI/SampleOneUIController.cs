using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleOneUIController : BaseUIController
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
            Debug.Debug("Load ui one");
            await base.OnStart(cancellation);

            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable ui one");
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            await base.OnDisable(closeImmediately);

            Debug.Debug("Disable ui one");

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