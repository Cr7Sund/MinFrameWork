using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Apis;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseGameController : UpdateController
    {
        [Inject(ServerBindDefine.GameTimer)] private IPromiseTimer _gameTimer;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog Debug;

        protected sealed override void OnStart()
        {
            OnSplashClosed();
        }

        protected sealed override void OnEnable()
        {
            GameStart();
        }

        protected override void OnStop()
        {
            GameOver();
            // PLAN UnBind Invoke dispose
        }

        protected override void OnUpdate(int millisecond)
        {
            _gameTimer.Update(millisecond);
        }

        #region  Login
        private void OnSplashClosed()
        {
            GameEnvInit();
        }

        private void GameEnvInit()
        {
            // InitSDK(); 
            // NetModule.Init();
            // InitHardware(); //_languageModule, Notch
            InitGameEnv();

        }


        private void GameStart()
        {
            HandleHotfix()
                .Then(RunLoginScene);
        }

        protected virtual void GameOver()
        {
            Console.Info("Game Over");
        }

        protected abstract void InitGameEnv();
        protected abstract IPromise HandleHotfix();
        protected abstract IPromise<INode> RunLoginScene();


        #endregion
    }
}