using System.Threading;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneOneController : BaseSceneController
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

        protected override async PromiseTask OnDisable()
        {
            Debug.Debug("Disable scene one");
            await base.OnDisable();
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