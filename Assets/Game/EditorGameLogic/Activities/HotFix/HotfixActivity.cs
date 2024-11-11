using Cr7Sund.LifeTime;
using UnityEngine;
namespace Cr7Sund.Game.Scene
{
    public class HotfixActivity : Activity
    {
        public static int StartValue;
        public static int EnableCount;

        protected async override PromiseTask OnCreateNode(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext)
        {
            await HandleHotfix();
            try
            {
                await FindNavController().Navigate(UI.EditorUIKeys.SampleOneUI);
            }
            catch (System.Exception ex)
            {
                Debug.Info(ex);
            }
        }


        private PromiseTask HandleHotfix()
        {
            return PromiseTask.CompletedTask;
        }


        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected async override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Load scene two");
            await base.OnStart(cancellation);
            StartValue++;

        }

        protected override async PromiseTask OnEnable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Enable scene two");
            await base.OnEnable(cancellation);
            EnableCount++;

        }

        protected override async PromiseTask OnDisable(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Disable scene two");
            await base.OnDisable(cancellation);
            EnableCount--;
        }

        protected override async PromiseTask OnStop(UnsafeCancellationToken cancellation)
        {
            Debug.Debug("Stop scene two");
            await base.OnStop(cancellation);
            StartValue--;
        }
    }
}
