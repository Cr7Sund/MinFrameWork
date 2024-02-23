using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;
using Cr7Sund.Package.Impl;
namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneNode : ModuleNode, ISceneNode
    {
        [Inject] private ISceneLoader _sceneLoader;

        public ISceneContainer SceneContainer { get; set; }


        protected override void OnInit()
        {
            base.OnInit();
            SceneContainer = new SceneContainer();
        }

        public override void Inject()
        {
            if (IsInjected)
                return;

            _context.InjectionBinder.Bind<ISceneNode>().To(this);
            base.Inject();
        }

        public override void DeInject()
        {
            if (IsInjected)
                return;

            _context.InjectionBinder.Unbind<INode>(this);
            base.DeInject();
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
                SceneContainer.SceneName = sceneKey;

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
                SceneContainer.SceneName = sceneKey;

                return _sceneLoader.LoadSceneAsync(sceneNode.Key, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad)
                                   .Then(() => _controllerModule.LoadAsync(content));

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

        protected override void OnDispose()
        {
            base.OnDispose();
            SceneContainer.Dispose();
        }
    }
}
