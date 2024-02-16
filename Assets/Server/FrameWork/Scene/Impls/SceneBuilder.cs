using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class SceneBuilder
    {
        protected SceneNode _node { get; private set; }
        private SceneContext _context;
        private ControllerModule _controllerModule;


        public void SetSceneKey(SceneKey key)
        {
            _node.Key = key;
        }
        public void BuildNode()
        {
            _node = CreateSceneNode();
            _node.AssignContext(_context);
            _node.AssignControllerModule(_controllerModule);

        }
        public void BuildContext()
        {
            _context = CreateContext();
        }
        public void BuildControllers()
        {
            _controllerModule = new ControllerModule();
            AddControllers(_controllerModule);
        }
        public SceneNode GetProduct()
        {
            return _node;
        }

        protected virtual SceneNode CreateSceneNode()
        {
            return new SceneNode();
        }
        protected abstract void AddControllers(IControllerModule controllerModule);
        protected abstract SceneContext CreateContext();

    }
}
