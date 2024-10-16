using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.AssetContainers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cr7Sund.AssetContainers
{
    public class SceneInstanceContainer : BaseInstancesContainer, ISceneInstanceContainer
    {
        [Inject] IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;
        public string SceneName { get; private set; }

        public void Init(string sceneName)
        {
            SceneName = sceneName;
        }

        public override void Dispose()
        {
            base.Dispose();
            SceneName = string.Empty;
        }

        protected override void OnCreate(GameObject instance)
        {
            var scene = SceneManager.GetSceneByName(SceneName);
            AssertUtil.NotNull(scene, AssetContainerExceptionType.create_from_invalidScene);
            SceneManager.MoveGameObjectToScene(instance, scene);
        }
    }
}