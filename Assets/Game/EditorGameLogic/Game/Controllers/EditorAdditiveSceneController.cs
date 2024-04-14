using System.Threading;
using Cr7Sund.Game.Scene;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorAdditiveSceneController : BaseController
    {
        [Inject] private ISceneModule _sceneModule;


        protected override async PromiseTask OnEnable()
        {
            await _sceneModule.AddScene(SceneKeys.EditorSceneKeyTwo);
        }

    }
}