using Cr7Sund.Game.Scene;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorMainController : UpdateController
    {
        [Inject] private ISceneModule _sceneModule;

        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("EditorMainController Start");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _sceneModule.AddScene(SceneKeys.EditorSceneKeyOne);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }


        protected override void OnUpdate(int millisecond)
        {
            // Log.Info("Update");
        }
    }
}