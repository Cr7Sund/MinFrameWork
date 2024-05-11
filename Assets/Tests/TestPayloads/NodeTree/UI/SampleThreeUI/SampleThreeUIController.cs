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

        protected override PromiseTask OnPrepare(object intentProPro)
        {
            if (Rejected)
            {
                throw new System.Exception("hello exception");
            }
            else
            {
                return promise.AsNewTask();
            }
        }
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override async PromiseTask OnStart(CancellationToken cancellation)
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

        protected override async PromiseTask OnDisable()
        {
            await base.OnDisable();
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