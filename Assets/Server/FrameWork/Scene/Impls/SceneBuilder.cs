using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class SceneBuilder
    {
        protected SceneNode _node { get; private set; }


        public void SetSceneKey(SceneKey key)
        {
            _node.Key = key;
        }
        public void BuildNode()
        {
            _node = CreateGameNode();
        }
        public void BuildContext()
        {
            var context = CreateContext();
            context.MapBindings();
            _node.AssignContext(context);
        }
        public void BuildControllers()
        {
            var controllerModule = new ControllerModule();
            AddControllers(controllerModule);
            _node.AssignControllerModule(controllerModule);
        }
        public SceneNode GetProduct()
        {
            return _node;
        }

        protected virtual SceneNode CreateGameNode()
        {
            return new SceneNode();
        }
        protected abstract void AddControllers(IControllerModule controllerModule);
        protected abstract SceneContext CreateContext();

    }
}
