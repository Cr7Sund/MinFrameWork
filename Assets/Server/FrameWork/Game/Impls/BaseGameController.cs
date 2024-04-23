using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Api;
using UnityEngine;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseGameController : BaseController, IUpdatable, ILateUpdate
    {
        [Inject(ServerBindDefine.GameTimer)] private IPromiseTimer _gameTimer;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog _log;
        [Inject(ServerBindDefine.GameInstancePool)] IInstancesContainer _gameInstanceContainer;
        [Inject] private IConfigContainer _configModule;

        [Inject] IAssetLoader _assetLoader;

        protected override IInternalLog Debug => _log;


        public void Update(int millisecond)
        {
            _gameTimer.Update(millisecond);
            OnUpdate(millisecond);
        }

        public void LateUpdate(int milliseconds)
        {
            OnLateUpdate(milliseconds);
        }

        protected sealed override async PromiseTask OnStart()
        {
            if (Application.isPlaying)
            {
                await _assetLoader.Init();
                await OnSplashClosed();
            }
        }

        protected override async PromiseTask OnEnable()
        {
            await GameStart();
        }

        protected override async PromiseTask OnStop()
        {
            if (Application.isPlaying)
            {
                await GameOver();
                await _assetLoader.DestroyAsync();
            }
        }

        #region  Login
        private async PromiseTask OnSplashClosed()
        {
            await InitConfig();
            await InitUI();
            await InitGameEnv();
        }

        private async PromiseTask GameStart()
        {
            await HandleHotfix();
            await RunLoginScene();
        }

        protected virtual PromiseTask GameOver()
        {
            _configModule.UnloadAll();
            _gameInstanceContainer.UnloadAll();
            Console.Info("Game Over");
            return PromiseTask.CompletedTask;
        }

        protected abstract PromiseTask InitGameEnv();
        protected abstract PromiseTask HandleHotfix();
        protected abstract PromiseTask RunLoginScene();
        protected virtual void OnUpdate(int millisecond) { }
        protected virtual void OnLateUpdate(int millisecond) { }

        #endregion

        #region  Init
        private async PromiseTask InitConfig()
        {
            var gameConfig = await _configModule.GetConfig<UIConfig>(ConfigDefines.UIConfig);
            foreach (var item in gameConfig.ConfigDefines)
            {
                await _configModule.GetConfig<Object>(item);
            }
        }

        private async PromiseTask InitUI()
        {
            await _gameInstanceContainer.InstantiateAsync<Object>(ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME);
        }
        #endregion
    }
}