using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.UI.Impl;
using Cr7Sund.Server.Apis;
namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneNode : ModuleNode, ISceneNode
    {
        [Inject] private ISceneLoader _sceneLoader;
        [Inject] private PageContainer _pageContainer;
        [Inject(ServerBindDefine.SceneTimer)] private IPromiseTimer _sceneTimer;


        public SceneNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        protected override void OnInit()
        {
            base.OnInit();
        }


        public IPromise ActiveScene()
        {
            if (Application.isPlaying)
            {
                return _sceneLoader.ActiveSceneAsync(Key);
            }
            else
            {
                return Promise.Resolved();
            }
        }

        protected override IPromise<INode> OnPreloadAsync(INode content)
        {
            var sceneNode = content as SceneNode;
            var sceneKey = (SceneKey)sceneNode.Key;

            if (sceneKey.IsVirtualScene)
            {
                return base.OnPreloadAsync(content);
            }
            else
            {
                return _sceneLoader.LoadSceneAsync(sceneNode.Key, sceneKey.LoadSceneMode, false)
                                   .Then(() => _controllerModule.LoadAsync(content));
            }
        }

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var sceneNode = content as SceneNode;
            var sceneKey = (SceneKey)sceneNode.Key;

            if (sceneKey.IsVirtualScene)
            {
                return base.OnLoadAsync(content);
            }
            else
            {
                return _sceneLoader.LoadSceneAsync(sceneNode.Key, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad)
                                   .Then(()=>_controllerModule.LoadAsync(content));
            }
        }

        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            var sceneNode = content as SceneNode;
            var sceneKey = (SceneKey)sceneNode.Key;

            if (!sceneKey.IsVirtualScene)
            {
                _sceneLoader.UnloadScene(sceneKey);
            }

            return base.OnUnloadAsync(content);
        }

        protected void CreateUITransitionBarrier()
        {
            _sceneTimer.Schedule((timeData) =>
            {
                _pageContainer.TimeOut(timeData.elapsedTime);
            });
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
