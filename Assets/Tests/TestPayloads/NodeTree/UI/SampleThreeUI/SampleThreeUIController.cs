using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleThreeUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        public static bool Rejected;

        public static IPromise promise;

        protected override PromiseTask OnPrepare(UnsafeCancellationToken cancellation, object intent)
        {
            if (Rejected)
            {
                throw new System.Exception("hello exception");
            }
            else
            {
                cancellation.Register(() =>
                {
                    if (promise.CurState == PromiseState.Pending)
                        promise.Cancel();
                });
                return promise.AsNewTask();
            }
        }
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            await base.OnStart(cancellation);
            Debug.Debug("Load ui three");

            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            await base.OnEnable();
            Debug.Debug("Enable ui three");

            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            await base.OnDisable(closeImmediately);
            Debug.Debug("Disable ui three");

            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            await base.OnStop();
            Debug.Debug("Stop ui three");

            StartValue--;
        }
    }
}