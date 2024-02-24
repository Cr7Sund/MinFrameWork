using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.Utils;
using UnityEngine;

namespace Cr7Sund.Server.Impl
{
    public abstract class GameBaseController : UpdateController
    {
        [Inject] private ISceneModule _sceneModule;
        [Inject] private IConfigContainer _configModule;
        [Inject] IAssetLoader _assetLoader;

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

        }

        #region  Login
        private void OnSplashClosed()
        {
            GameEnvInit();
        }

        private void GameEnvInit()
        {
            InitConfig();

            // InitSDK(); 
            // NetModule.Init();
            // InitHardware(); //_languageModule, Notch
            InitGameEnv();
        }

        private void InitConfig()
        {
            var gameConfig = _assetLoader.LoadSync<UIConfig>(ConfigDefines.UIConfig);
            foreach (var item in gameConfig.ConfigDefines)
            {
                _configModule.GetConfig<Object>(item);
            }
        }

        private void GameStart()
        {
            HandleHotfix()
                .Then(RunLoginScene);
        }

        protected virtual void GameOver()
        {
            Debug.Info("Game Over");
        }

        protected abstract void InitGameEnv();
        protected abstract IPromise HandleHotfix();
        protected abstract IPromise<INode> RunLoginScene();


        #endregion
    }
}