using Cr7Sund.PackageTest.IOC;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Selector.Impl
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