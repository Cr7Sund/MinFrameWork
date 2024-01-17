using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Server.Impl
{
    public abstract class GameBuilder
    {
        protected GameNode _node { get; private set; }
        private GameContext _context;
        private ControllerModule _controllerModule;


        public GameNode BuildNode()
        {
            _node = CreateGameNode();

            _node.AssignContext(_context);
            _node.AssignControllerModule(_controllerModule);

            return _node;
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

        public GameNode GetProduct()
        {
            return _node;
        }


        protected abstract GameContext CreateContext();

        protected virtual GameNode CreateGameNode()
        {
            return new GameNode();
        }

        protected virtual void AddControllers(IControllerModule controllerModule)
        {

        }
    }
}
