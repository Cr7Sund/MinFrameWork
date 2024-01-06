using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public class GameBuilder
    {
        protected GameNode _node { get; private set; }



        public GameNode BuildNode()
        {
            _node = CreateGameNode();
            return _node;
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

        public GameNode GetProduct()
        {
            return _node;
        }

        protected virtual GameNode CreateGameNode()
        {
            return new GameNode();
        }

        protected virtual GameContext CreateContext()
        {
            return new GameContext();
        }

        protected virtual void AddControllers(IControllerModule controllerModule)
        {

        }
    }
}
