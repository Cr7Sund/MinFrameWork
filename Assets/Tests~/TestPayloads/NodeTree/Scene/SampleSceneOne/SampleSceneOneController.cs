using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneOneController : BaseSceneController
    {
        public static int StartValue;
        public static int EnableCount;
        public static IPromise LoadPromise;

        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
            LoadPromise = Promise.Resolved();
        }
        protected override async PromiseTask OnPrepare(UnsafeCancellationToken cancellation)
        {
            cancellation.Register(() => LoadPromise?.Cancel());
            await LoadPromise.Join();
            await base.OnPrepare(cancellation);
        }

        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load scene one");
            await base.OnStart(cancellation);
            StartValue += 2;
        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable scene one");
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(bool closeImmediately)
        {
            Debug.Debug("Disable scene one");
            await base.OnDisable(closeImmediately);
            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop scene one");
            await base.OnStop();
            StartValue--;
        }
    }
}