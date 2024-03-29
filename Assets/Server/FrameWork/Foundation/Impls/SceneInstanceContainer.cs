using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Apis;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public class SceneInstanceContainer : InstanceContainer, ISceneInstanceContainer
    {
        [Inject] IAssetLoader _assetLoader;


        protected override IAssetLoader _loader => _assetLoader;
        public string SceneName { get; private set; }


        public void Init(string sceneName)
        {
            SceneName = sceneName;
        }

        protected override void MoveInstanceToScene(GameObject instance)
        {
            var scene = SceneManager.GetSceneByName(SceneName);
            AssertUtil.NotNull(scene, FoundationExceptionType.create_from_invalidScene);
            SceneManager.MoveGameObjectToScene(instance, scene);
        }

        public override void Dispose()
        {
            base.Dispose();
            SceneName = string.Empty;
        }
    }
}