using Cr7Sund.Game.Scene;
using Cr7Sund.AssetContainers;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : BaseGameController
    {

        #region Login
        protected override PromiseTask InitGameEnv()
        {
            Debug.Debug("EditorMainController Start");
            return PromiseTask.CompletedTask;
        }

        protected override PromiseTask HandleHotfix()
        {
            return PromiseTask.CompletedTask;
        }

        protected override async PromiseTask RunLoginScene()
        {
            await _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
        }

        #endregion

        #region Exit Game
        protected override async PromiseTask OnGameStop()
        {
            await _sceneModule.UnloadScene(SceneKeys.EditorSceneKeyOne);
            await base.OnGameStop();
        }

        #endregion

        protected override void OnUpdate(int millisecond)
        {
            base.OnUpdate(millisecond);
        }

 
    }
}
