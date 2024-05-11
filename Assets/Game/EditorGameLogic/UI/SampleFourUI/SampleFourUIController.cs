using System.Threading;
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


        protected override async PromiseTask OnStart(CancellationToken cancellation)
        {
            Debug.Debug("Load ui four");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable()
        {
            Debug.Debug("Enable ui four");
            await base.OnEnable();
            EnableCount++;
        }

        protected override async PromiseTask OnDisable()
        {
            Debug.Debug("Disable ui four");
            await base.OnDisable();
            EnableCount--;
        }

        protected override async PromiseTask OnStop()
        {
            Debug.Debug("Stop ui four");
            await base.OnStop();
            StartValue--;
        }
    }
}