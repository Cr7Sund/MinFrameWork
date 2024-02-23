using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleThreeUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        public static IPromise promise;
        protected override IPromise OnPrepare(object intent)
        {
            return promise;
        }
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("Load ui three");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Info("Enable ui three");

            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Info("Disable ui three");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Info("Stop ui three");

            StartValue--;
        }
    }
}