namespace Cr7Sund.Server.Impl
{
    public class SceneBuilder
    {
        protected SceneNode _node { get; private set; }


        internal void SetSceneKey(SceneKey key)
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
            var controllerModule = GetControllerModule();
            controllerModule.AddInternalControllers();
            _node.ControllerModule = controllerModule;
        }

        public SceneNode GetProduct()
        {
            return _node;
        }

        protected virtual SceneNode CreateGameNode()
        {
            return new SceneNode();
        }
        protected virtual SceneModule GetControllerModule()
        {
            return new SceneModule();
        }
        protected virtual SceneContext CreateContext()
        {
            return new SceneContext();
        }
    }
}
