using Cr7Sund.LifeTime;

namespace Cr7Sund.Game.UI
{
    public class SampleTwoUIController : Fragment
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
            Debug.Debug("Load ui two");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable ui two");
            await base.OnEnable(cancellation);
            EnableCount++;
        }

        protected override async PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Disable ui two");
            await base.OnDisable(cancellation);
            EnableCount--;
        }

        protected override async PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Stop ui two");
            await base.OnStop(cancellation);
            StartValue--;
        }
    }
}