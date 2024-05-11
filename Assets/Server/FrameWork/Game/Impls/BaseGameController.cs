using System.Threading;
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
        [Inject(ServerBindDefine.GameInstancePool)] IInstancesContainer _gameInstanceContainer;
        [Inject] private IConfigContainer _configModule;
        [Inject] private UI.Api.IUITransitionAnimationContainer _uiTransModule;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private ISceneLoader _sceneLoader;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog _log;

        protected override IInternalLog Debug => _log;


        public void Update(int millisecond)
        {
            _gameTimer.Update(millisecond);
            OnUpdate(millisecond);
        }

        public void LateUpdate(int milliseconds)
        {
            _sceneLoader.LateUpdate(milliseconds);
            OnLateUpdate(milliseconds);
        }

        protected sealed override async PromiseTask OnStart(CancellationToken cancellation)
        {
            if (Application.isPlaying)
            {
                await _assetLoader.Init();
                _sceneLoader.Init();
                await OnSplashClosed(cancellation);
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
        private async PromiseTask OnSplashClosed(CancellationToken cancellation)
        {
            await InitConfig(cancellation);
            await InitUI(cancellation);
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
            _uiTransModule.UnloadAll();
            _gameInstanceContainer.UnloadAll();

            return PromiseTask.CompletedTask;
        }

        protected abstract PromiseTask InitGameEnv();
        protected abstract PromiseTask HandleHotfix();
        protected abstract PromiseTask RunLoginScene();
        protected virtual void OnUpdate(int millisecond) { }
        protected virtual void OnLateUpdate(int millisecond) { }

        #endregion

        #region  Init
        private async PromiseTask InitConfig(CancellationToken cancellation)
        {
            var gameConfig = await _configModule.LoadAssetAsync<UIConfig>(ConfigDefines.UIConfig, cancellation);
            foreach (var item in gameConfig.ConfigDefines)
            {
                await _configModule.LoadAssetAsync<Object>(item, cancellation);
            }
        }

        private async PromiseTask InitUI(CancellationToken cancellation)
        {
            await _gameInstanceContainer.InstantiateAsync<Object>(ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME, cancellation);
        }
        #endregion
    }
}