using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseGameController : UpdateController
    {
        [Inject(ServerBindDefine.GameTimer)] private IPromiseTimer _gameTimer;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog Debug;

        protected sealed override async PromiseTask OnStart()
        {
           await OnSplashClosed();
        }

        protected override async PromiseTask OnEnable()
        {
            await GameStart();
        }

        protected override async PromiseTask OnStop()
        {
            GameOver();
            // PLAN UnBind Invoke dispose

            await base.OnStop();
        }

        protected override void OnUpdate(int millisecond)
        {
            _gameTimer.Update(millisecond);
        }

        #region  Login
        private async PromiseTask OnSplashClosed()
        {
            await InitGameEnv();
        }

        private async PromiseTask GameStart()
        {
            await HandleHotfix();
            await RunLoginScene();
        }

        protected virtual void GameOver()
        {
            Console.Info("Game Over");
        }

        protected abstract PromiseTask InitGameEnv();
        protected abstract PromiseTask HandleHotfix();
        protected abstract PromiseTask RunLoginScene();

        #endregion
    }
}