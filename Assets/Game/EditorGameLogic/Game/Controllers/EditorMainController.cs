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
            try
            {
                await _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
            }
            catch (Exception ex)
            {
                Console.Error(ex);
            }
        }



        #endregion

        #region Exit Game
        protected override async PromiseTask GameOver()
        {
            await base.GameOver();
            await _sceneModule.UnloadScene(SceneKeys.EditorSceneKeyOne);
        }

        #endregion

        protected override void OnUpdate(int millisecond)
        {
            base.OnUpdate(millisecond);
        }
    }
}
