using Cr7Sund.Game.Scene;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : GameBaseController
    {
        [Inject] private ISceneModule _sceneModule;


        #region  Login

        protected override void InitGameEnv()
        {
            Debug.Info("EditorMainController Start");
        }

        protected override IPromise HandleHotfix()
        {
            return Promise.Resolved();
        }

        protected override IPromise<INode> RunLoginScene()
        {
            return _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
        }


        #endregion
    }
}