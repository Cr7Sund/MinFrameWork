using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.Game.Scene;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : BaseGameController
    {
        [Inject] private ISceneModule _sceneModule;
        [Inject] private IConfigContainer _configModule;
        [Inject(ServerBindDefine.GameInstancePool)] IInstancesContainer _gameInstanceContainer;


        #region Login
        protected override async PromiseTask InitGameEnv()
        {
            await InitConfig();
            InitUI();
            Debug.Debug("EditorMainController Start");
        }

        protected override PromiseTask HandleHotfix()
        {
            return PromiseTask.CompletedTask;
        }

        protected override async PromiseTask RunLoginScene()
        {
            // var handler = Addressables.LoadAssetAsync<Object>(SampleUIKeys.SampleOneUI.Key);
            // var assetPromise = new AssetPromise();
            // assetPromise.Then(asset =>
            // {
            //     GameObject.Instantiate(asset);
            // });
            // handler.ToPromise<Object>(assetPromise);
            // await assetPromise.AsNewTask();
            await _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
        }

        private async PromiseTask InitConfig()
        {
            // PLAN add asset loader res count checking
            // Or pass the access to the assetLoader directly
            var gameConfig = await _configModule.GetConfig<UIConfig>(ConfigDefines.UIConfig);
            foreach (var item in gameConfig.ConfigDefines)
            {
                await _configModule.GetConfig<UnityEngine.Object>(item);
            }

        }

        private void InitUI()
        {
            _gameInstanceContainer.Instantiate<Object>(ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME);
        }
        #endregion

        #region Exit Game
        protected override void GameOver()
        {
            base.GameOver();

        }
        #endregion

        protected override void OnUpdate(int millisecond)
        {
            base.OnUpdate(millisecond);
        }
    }
}
