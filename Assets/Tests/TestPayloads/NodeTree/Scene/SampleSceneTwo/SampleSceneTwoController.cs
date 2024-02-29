using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneTwoController : BaseSceneController
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
            Debug.Debug("Load scene two");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable scene two");

            EnableCount++;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable scene two");

            EnableCount--;

        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop scene two");

            StartValue--;
        }
    }
}