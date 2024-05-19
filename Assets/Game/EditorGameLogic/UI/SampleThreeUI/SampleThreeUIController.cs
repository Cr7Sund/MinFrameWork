using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleThreeUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        public static IPromise promise;
        protected override PromiseTask OnPrepare(UnsafeCancellationToken cancellation, object intent)
        {
            return promise.AsNewTask();
        }
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }

        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load ui three");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable ui three");
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            Debug.Debug("Disable ui three");
            await base.OnDisable(closeImmediately);
            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui three");
            await base.OnStop();
            StartValue--;
        }
    }
}