using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public class GameBuilder
    {
        protected GameNode _node { get; private set; }



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

        public GameNode GetProduct()
        {
            return _node;
        }

        protected virtual GameNode CreateGameNode()
        {
            return new GameNode();
        }
        protected virtual GameModule GetControllerModule()
        {
            return new GameModule();
        }
        protected virtual GameContext CreateContext()
        {
            return new GameContext();
        }
    }
}
