using System.Threading;
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


        protected override async PromiseTask OnStart()
        {
            Debug.Debug("Load scene two");
            await base.OnStart();
            StartValue++;
        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable scene two");
            await base.OnEnable();

            EnableCount++;

        }

        protected override async PromiseTask OnDisable()
        {
            Debug.Debug("Disable scene two");
            await base.OnDisable();

            EnableCount--;

        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop scene two");
            await base.OnStop();
            StartValue--;
        }
    }
}