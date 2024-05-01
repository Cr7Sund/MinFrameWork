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
            CreateUITransitionBarrier();
        }

        public async override PromiseTask OnEnable()
        {
            await base.OnEnable();
            await ActiveScene();
        }

        protected override async PromiseTask OnPreloadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, false);
            }
            await base.OnPreloadAsync();
        }

        protected override async PromiseTask OnLoadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad);
            }
            await base.OnLoadAsync();
        }

        protected async override PromiseTask OnUnloadAsync()
        {
            await _pageModule.CloseAll();

            var sceneKey = (SceneKey)Key;
            if (!sceneKey.IsVirtualScene)
            {
                _sceneLoader.UnloadScene(sceneKey);
            }
            await base.OnUnloadAsync();
        }

        public override void RegisterAddTask(CancellationToken cancellationToken)
        {
            base.RegisterAddTask(cancellationToken);
            _sceneLoader.RegisterCancelLoad(Key, cancellationToken);
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
