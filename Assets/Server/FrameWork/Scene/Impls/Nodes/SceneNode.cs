using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using Cr7Sund.Server.UI.Api;
using System.Threading;

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

        protected override void OnInit()
        {
            base.OnInit();
        }

        public override PromiseTask OnStart()
        {
            CreateUITransitionBarrier();
            return base.OnStart();
        }

        public async override PromiseTask OnEnable()
        {
            await base.OnEnable();
            await ActiveScene();
        }

        public override async PromiseTask OnStopAsync()
        {
            await _pageModule.CloseAll();
            _sceneTimer.Clear();
            await base.OnStopAsync();
        }

        protected override async PromiseTask OnPreloadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, false, AddCancellation.Token);
            }
            await base.OnPreloadAsync();
        }

        protected override async PromiseTask OnLoadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad, AddCancellation.Token);
            }
            await base.OnLoadAsync();
        }

        protected override async PromiseTask OnCancelLoadAsync(CancellationToken cancellation)
        {
            await _sceneLoader.RegisterCancelLoad(Key, cancellation);
            await base.OnCancelLoadAsync(cancellation);
        }

        protected async override PromiseTask OnUnloadAsync()
        {
            var sceneKey = (SceneKey)Key;
            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.UnloadScene(sceneKey);
            }
            
            await base.OnUnloadAsync();
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

        private void CreateUITransitionBarrier()
        {
            _sceneTimer.Schedule((timeData) =>
                    {
                        _pageModule.TimeOut(timeData.elapsedTime);
                    });
        }
    }
}
