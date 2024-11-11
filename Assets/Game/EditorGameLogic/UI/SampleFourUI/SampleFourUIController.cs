
using Cr7Sund.LifeTime;
namespace Cr7Sund.Game.UI
{
    public class SampleFourUIController : Fragment
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
            Debug.Debug("Load ui four");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable ui four");
            await base.OnEnable(cancellation);
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Disable ui four");
            await base.OnDisable(cancellation);
            EnableCount--;
        }

        protected override async PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Stop ui four");
            await base.OnStop(cancellation);
            StartValue--;
        }
    }
}