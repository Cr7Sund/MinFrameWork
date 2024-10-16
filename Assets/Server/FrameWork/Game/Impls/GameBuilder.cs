using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.AssetContainers
{

    public abstract class GameBuilder 
    {
        protected GameNode _node { get; private set; }
        private GameContext _context;
        private ControllerModule _controllerModule;


        public void BuildNode()
        {
            _node = CreateGameNode();

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _controllerModule.AddController(CreateController(), default);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

        protected abstract BaseGameController CreateController();

    }
}
