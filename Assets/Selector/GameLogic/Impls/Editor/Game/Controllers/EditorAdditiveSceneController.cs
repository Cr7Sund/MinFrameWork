using Cr7Sund.Framework.Tests;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Apis;
using UnityEngine.PlayerLoop;

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