using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Config;
using Cr7Sund.Game.Scene;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Apis;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : BaseGameController
    {
        [Inject] private ISceneModule _sceneModule;
        [Inject] private IConfigContainer _configModule;
        [Inject(ServerBindDefine.GameInstancePool)] IInstanceContainer _gameInstanceContainer;


        #region  Login

        protected override void InitGameEnv()
        {
            InitConfig();
            InitUI();
            Debug.Debug("EditorMainController Start");
        }

        protected override IPromise HandleHotfix()
        {
            return Promise.Resolved();
        }

        protected override IPromise<INode> RunLoginScene()
        {
            return _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
        }

        private void InitConfig()
        {
            // PLAN add asset loader res count checking
            // Or pass the access to the assetLoader directly
            var gameConfig = _configModule.GetConfig<UIConfig>(ConfigDefines.UIConfig);
            foreach (var item in gameConfig.ConfigDefines)
            {
                _configModule.GetConfig<UnityEngine.Object>(item);
            }

        }

        private void InitUI()
        {
            _gameInstanceContainer.Instantiate(ServerBindDefine.UIRootAssetKey, ServerBindDefine.UI_ROOT_NAME);
        }
        
        #endregion

        #region  Exit Game
        protected override void GameOver()
        {
            base.GameOver();

        }
        #endregion
    }
}