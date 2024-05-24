using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.AssetLoader.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneNode : ModuleNode, ISceneNode
    {
        [Inject] private ISceneLoader _sceneLoader;


        public SceneNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        public async override PromiseTask OnEnable()
        {
            await base.OnEnable();
            await ActiveScene();
        }

        protected override async PromiseTask OnPreloadAsync(UnsafeCancellationToken cancellation)
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, false, cancellation);
            }
            await base.OnPreloadAsync(cancellation);
        }

        protected override async PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation)
        {
            var sceneKey = (SceneKey)Key;

            var promiseTask = base.OnPreloadAsync(cancellation);
            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad, cancellation);
            }
            await promiseTask;
            await base.OnLoadAsync(cancellation);
        }

        protected async override PromiseTask OnUnloadAsync(UnsafeCancellationToken cancellation)
        {
            var sceneKey = (SceneKey)Key;
            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.UnloadScene(sceneKey);
            }

            await base.OnUnloadAsync(cancellation);
        }

        private PromiseTask ActiveScene()
        {
            var sceneKey = Key as SceneKey;
            if (!sceneKey.IsVirtualScene)
            {
                return _sceneLoader.ActiveSceneAsync(Key);
            }
            else
            {
                return PromiseTask.CompletedTask;
            }
        }
    }
}
