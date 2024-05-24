using Cr7Sund.Package.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    internal class InternalSceneController : BaseSceneController
    {
        [Inject(ServerBindDefine.SceneTimer)] private IPromiseTimer _sceneTimer;
        [Inject] IPageModule _pageModule;


        protected override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            CreateUITransitionBarrier(cancellation);
            return base.OnStart(cancellation);
        }

        protected override void OnUpdate(int milliseconds)
        {
            _sceneTimer.Update(milliseconds);
            base.OnUpdate(milliseconds);
        }

        protected override async PromiseTask OnStop()
        {
            await base.OnStop();
            await _pageModule.CloseAll();
            _sceneTimer.Clear();
        }

        private void CreateUITransitionBarrier(UnsafeCancellationToken cancellation)
        {
            _sceneTimer.Schedule((timeData) =>
                    {
                        _pageModule.TimeOut(timeData.elapsedTime);
                    }, cancellation);
        }
    }
}