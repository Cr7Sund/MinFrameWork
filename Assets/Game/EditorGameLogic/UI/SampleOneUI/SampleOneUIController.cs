

using Cr7Sund.LifeTime;
namespace Cr7Sund.Game.UI
{
    public class SampleOneUIController : Fragment
    {
        public int StartValue;
        public int EnableCount;



        protected override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load ui one {StartValue}", StartValue);
            await base.OnStart(cancellation);
            StartValue++;
        }

        protected override async PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable ui one");

            try
            {
                await FindNavController().Navigate(EditorUIKeys.SampleTwoUI);
                await FindNavController().Navigate(EditorUIKeys.SampleTwoUI);
            }
            catch (System.Exception ex)
            {
                Debug.Info(ex);
            }
            await base.OnEnable(cancellation);

            EnableCount++;
        }

        protected override async PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Disable ui one");
            await base.OnDisable(cancellation);
            EnableCount--;
        }

        protected override async PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Stop ui one");
            await base.OnStop(cancellation);
            StartValue--;
        }
    }
}
