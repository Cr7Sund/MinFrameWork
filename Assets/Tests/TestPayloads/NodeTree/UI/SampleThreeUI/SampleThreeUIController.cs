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

        protected override IPromise OnPrepare(object intent = null)
        {
            if (Rejected)
            {
                throw new System.Exception();
            }
            else
            {
                return promise;
            }
        }
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override void OnStart()
        {
            base.OnStart();
            Debug.Debug("Load ui three");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable ui three");

            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable ui three");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop ui three");

            StartValue--;
        }
    }
}