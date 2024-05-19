using System.Threading;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Scene.Apis;
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
        [Inject] private IPoolBinder _poolBinder;
        [Inject(ServerBindDefine.GameLogger)] protected IInternalLog _log;
        [Inject] protected ISceneModule _sceneModule;


        protected override IInternalLog Debug => _log ?? Console.Logger;


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

        protected sealed override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            await _assetLoader.Init();
            _sceneLoader.Init();
            CreateSceneTransitionBarrier(cancellation);
            await OnGameStart(cancellation);
        }

        protected override async PromiseTask OnEnable()
        {
            await OnGameEnable();
        }

        protected override async PromiseTask OnStop()
        {
            await OnGameStop();
            await _assetLoader.DestroyAsync();
            _gameTimer.Clear();
        }

        #region  Start Game
        private async PromiseTask OnGameStart(UnsafeCancellationToken cancellation)
        {
            await InitConfig(cancellation);
            await InitUI(cancellation);
            await InitGameEnv();
        }

        private async PromiseTask OnGameEnable()
        {
            await HandleHotfix();
            await RunLoginScene();
        }

        protected virtual async PromiseTask OnGameStop()
        {
            await _configModule.UnloadAll();
            await _uiTransModule.UnloadAll();
            await _gameInstanceContainer.UnloadAll();
        }

        protected abstract PromiseTask InitGameEnv();
        protected abstract PromiseTask HandleHotfix();
        protected abstract PromiseTask RunLoginScene();
        protected virtual void OnUpdate(int millisecond) { }
        protected virtual void OnLateUpdate(int millisecond) { }

        #endregion

        #region  Init
        private async PromiseTask InitConfig(UnsafeCancellationToken cancellation)
        {
            var gameConfig = await _configModule.LoadAssetAsync<UIConfig>(ConfigDefines.UIConfig, cancellation);
            await _configModule.LoadGroup(gameConfig.ConfigDefines, cancellation, _poolBinder);
        }

        private async PromiseTask InitUI(UnsafeCancellationToken cancellation)
        {
            await _gameInstanceContainer.InstantiateAsync<Object>(ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME, cancellation);
        }
        #endregion

        private void CreateSceneTransitionBarrier(UnsafeCancellationToken cancellation)
        {
            _gameTimer.Schedule((timeData) =>
                    {
                        _sceneModule.TimeOut(timeData.elapsedTime);
                    }, cancellation);
        }
    }
}