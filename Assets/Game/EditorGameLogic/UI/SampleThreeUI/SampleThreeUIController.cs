using System.Threading;
using Cr7Sund.LifeTime;

namespace Cr7Sund.Game.UI
{
    public class SampleThreeUIController : Fragment
    {
        public static int StartValue;
        public static int EnableCount;

  
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }

        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load ui three");
            await base.OnStart(cancellation);
            StartValue++;
        }

        protected override async PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable ui three");
            await base.OnEnable(cancellation);
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Disable ui three");
            await base.OnDisable(cancellation);
            EnableCount--;
        }

        protected override async PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Stop ui three");
            await base.OnStop(cancellation);
            StartValue--;
        }
    }
}