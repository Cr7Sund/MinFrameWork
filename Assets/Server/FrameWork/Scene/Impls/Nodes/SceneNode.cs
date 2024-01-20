using Cr7Sund.Framework.Api;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Apis;
using UnityEngine;
namespace Cr7Sund.Server.Impl
{
    public partial class SceneNode : ModuleNode
    {
        [Inject] private ISceneLoader _sceneLoader;

        public ISceneKey Key { get; set; }


        internal void AssignContext(IContext context)
        {
            _context = context;
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            if (MacroDefine.IsEditor && !Application.isPlaying)
            {
                return base.OnLoadAsync(content);
            }
            else
            {
                var sceneNode = content as SceneNode;
                return _sceneLoader.LoadSceneAsync(sceneNode.Key, 
                            sceneNode.Key.LoadSceneMode, sceneNode.Key.ActivateOnLoad)
                            .Then(() => _controllerModule.LoadAsync(content));

            }

        }
    }
}
