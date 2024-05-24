using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.AssetLoader.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneNode : ModuleNode, ISceneNode
    {
        [Inject] private ISceneLoader _sceneLoader;
        [Inject(ServerBindDefine.SceneTimer)] private IPromiseTimer _sceneTimer;
        [Inject] IPageModule _pageModule;

        public SceneNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        public override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            CreateUITransitionBarrier(cancellation);
            return base.OnStart(cancellation);
        }

        public async override PromiseTask OnEnable()
        {
            await base.OnEnable();
            await ActiveScene();
        }

        protected override void OnUpdate(int milliseconds)
        {
            _sceneTimer.Update(milliseconds);
            base.OnUpdate(milliseconds);
        }

        public override async PromiseTask OnStop()
        {
            await base.OnStop();
            await _pageModule.CloseAll();
            _sceneTimer.Clear();
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

        private void CreateUITransitionBarrier(UnsafeCancellationToken cancellation)
        {
            _sceneTimer.Schedule((timeData) =>
                    {
                        _pageModule.TimeOut(timeData.elapsedTime);
                    }, cancellation);
        }
    }
}
