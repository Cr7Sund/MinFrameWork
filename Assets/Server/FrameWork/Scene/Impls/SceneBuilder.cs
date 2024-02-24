using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class SceneBuilder
    {
        protected SceneNode _node { get; private set; }
        private SceneContext _context;
        private ControllerModule _controllerModule;


        public void BuildNode(SceneKey sceneKey)
        {
            _node = CreateSceneNode(sceneKey);
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

        protected virtual SceneNode CreateSceneNode(SceneKey sceneKey)
        {
            return new SceneNode(sceneKey);
        }
        protected abstract void AddControllers(IControllerModule controllerModule);
        protected abstract SceneContext CreateContext();

    }
}
