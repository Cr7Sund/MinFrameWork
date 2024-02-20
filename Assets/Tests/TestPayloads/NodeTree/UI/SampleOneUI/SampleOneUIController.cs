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


        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("Load ui one");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Info("Enable ui one");

            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Info("Disable ui one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Info("Stop ui one");

            StartValue--;
        }
    }
}