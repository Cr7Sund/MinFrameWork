using Cr7Sund.NodeTree.Impl;
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



        protected override void OnStart()
        {
            base.OnStart();
            Debug.Debug("Load scene one");

            StartValue +=2;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable scene one");


            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable scene one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop scene one");

            StartValue--;
        }
    }
}