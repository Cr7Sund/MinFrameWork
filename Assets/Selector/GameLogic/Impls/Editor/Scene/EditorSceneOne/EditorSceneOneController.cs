using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class EditorSceneOneController : BaseController
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
            Debug.Info("Load scene one");

            StartValue++;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Info("Enable scene one");


            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Info("Disable scene one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Info("Stop scene one");

            StartValue--;
        }
    }
}