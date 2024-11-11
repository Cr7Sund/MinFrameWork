using Cr7Sund.LifeTime;

namespace Cr7Sund.Game.Scene
{
    public class LoginActivity : Activity
    {

        protected async override PromiseTask OnNodeCreated(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext)
        {
            try
            {
                await FindNavController() .Navigate(UI.EditorUIKeys.SampleOneUI);
            }
            catch (System.Exception ex)
            {
                Debug.Info(ex);
            }
        }



        protected async override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            await base.OnStart(cancellation);
            Debug.Debug("Load scene one");
        }

        protected async override PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable scene one");
        }

        protected async override PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            await base.OnDisable(cancellation);
            Debug.Debug("Disable scene one");
        }

        protected async override PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            await base.OnStop( cancellation);
            Debug.Debug("Stop scene one");
        }

    }

}
