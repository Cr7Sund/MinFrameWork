using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;
using Cr7Sund.Server.UI.Api;

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

        public async override PromiseTask OnStop()
        {
            await base.OnStop();
            await _pageModule.CloseAll();
        }

        protected override async PromiseTask OnPreloadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey, sceneKey.LoadSceneMode, false);
            }
        }

        protected override async PromiseTask OnLoadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                await _sceneLoader.LoadSceneAsync(sceneKey);
            }
        }

        protected override PromiseTask OnUnloadAsync()
        {
            var sceneKey = (SceneKey)Key;

            if (!sceneKey.IsVirtualScene)
            {
                _sceneLoader.UnloadScene(sceneKey);
            }

            return base.OnUnloadAsync();
        }

        private PromiseTask ActiveScene()
        {
            if (MacroDefine.IsMainThread && Application.isPlaying)
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
