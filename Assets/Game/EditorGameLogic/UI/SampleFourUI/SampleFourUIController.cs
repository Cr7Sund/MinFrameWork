using Cr7Sund.Server.UI.Impl;
using UnityEngine;

namespace Cr7Sund.Game.UI
{
    public class SampleFourUIController : BaseUIController
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
            Debug.Debug("Load ui four");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable ui four");

            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable ui four");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop ui four");

            StartValue--;
        }
    }
}