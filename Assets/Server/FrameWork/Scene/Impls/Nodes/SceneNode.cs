using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Apis;
using UnityEngine;
namespace Cr7Sund.Server.Impl
{
    public class SceneNode : ModuleNode, ISceneNode
    {
        [Inject] private ISceneLoader _sceneLoader;


        public override void Inject()
        {
            if (IsInjected)
                return;

            base.Inject();
            _context.InjectionBinder.Bind<ISceneNode>().To(this);
        }

        public override void DeInject()
        {
            if (IsInjected)
                return;

            _context.InjectionBinder.Unbind<INode>(this);
            base.DeInject();
        }
        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var sceneNode = content as SceneNode;
            var sceneKey = sceneNode.Key as SceneKey;

            if (sceneKey.IsVirtualScene)
            {
                return base.OnLoadAsync(content);
            }
            else
            {
                return _sceneLoader.LoadSceneAsync(sceneNode.Key, sceneKey.LoadSceneMode, sceneKey.ActivateOnLoad)
                                   .Then(() => _controllerModule.LoadAsync(content));

            }
        }


    }
}
