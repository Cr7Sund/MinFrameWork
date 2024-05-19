using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class PanelNode : UINode
    {
        [Inject] IPanelModule _panelModule;
        [Inject(ServerBindDefine.UITimer)] private IPromiseTimer _uiTimer;

        public PanelNode(IAssetKey assetKey, IUIView uiView, IUIController uiController) : base(assetKey, uiView, uiController)
        {

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