
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PageNode : UINode
    {
        [Inject] IPanelModule _panelModule;
        [Inject(ServerBindDefine.UITimer)] private IPromiseTimer _uiTimer;


        public PageNode(IAssetKey assetKey, IUIView uiView, IUIController uiController) : base(assetKey, uiView, uiController)
        {

        }


        protected override async PromiseTask OnUnloadAsync(UnsafeCancellationToken cancellation)
        {
            await _panelModule.CloseAll();
            await base.OnUnloadAsync(cancellation);
        }

        public override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            CreateUITransitionBarrier(cancellation);
            return base.OnStart(cancellation);
        }

        public override async PromiseTask OnStop()
        {
            await base.OnStop();
            await _panelModule.CloseAll();
            _uiTimer.Clear();
        }

        private void CreateUITransitionBarrier(UnsafeCancellationToken cancellation)
        {
            _uiTimer.Schedule((timeData) =>
                    {
                        _panelModule.TimeOut(timeData.elapsedTime);
                    }, cancellation);
        }
    }
}