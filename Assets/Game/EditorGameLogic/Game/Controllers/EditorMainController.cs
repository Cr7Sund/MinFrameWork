using System;
using Cr7Sund.Game.Scene;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : BaseGameController
    {
        [Inject] private ISceneModule _sceneModule;


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
        protected override async PromiseTask GameOver()
        {
            await _sceneModule.UnloadScene(SceneKeys.EditorSceneKeyOne);
            await base.GameOver();
        }

        #endregion

        protected override void OnUpdate(int millisecond)
        {
            base.OnUpdate(millisecond);
        }
    }
}
