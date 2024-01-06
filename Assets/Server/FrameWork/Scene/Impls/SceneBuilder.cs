using System;

namespace Cr7Sund.Server.Impl
{
    public abstract class SceneBuilder
    {
        protected SceneNode _node { get; private set; }


        public SceneBuilder()
        {
            SubClassInitialization();
        }


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

        public SceneNode GetProduct()
        {
            return _node;
        }
        public abstract void SubClassInitialization();

        protected virtual SceneNode CreateGameNode()
        {
            return new SceneNode();
        }

        protected virtual SceneContext CreateContext()
        {
            return new SceneContext();
        }

    }
}
