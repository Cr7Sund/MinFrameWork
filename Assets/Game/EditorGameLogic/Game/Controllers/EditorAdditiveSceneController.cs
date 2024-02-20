using Cr7Sund.Game.Scene;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorAdditiveSceneController : BaseController
    {
        [Inject] private ISceneModule _sceneModule;


        protected override void OnEnable()
        {
            base.OnEnable();
            _sceneModule.AddScene(SceneKeys.EditorSceneKeyTwo);
        }

    }
}