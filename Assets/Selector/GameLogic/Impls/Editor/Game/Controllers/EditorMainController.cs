using Cr7Sund.Framework.Tests;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Apis;
using UnityEngine.PlayerLoop;

namespace Cr7Sund.Selector.Impl
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