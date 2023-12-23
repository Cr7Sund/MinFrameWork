using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Server.Impl
{
    public class SceneNode : Node
    {
        public SceneKey Key { get; set; }

        internal IControllerModule ControllerModule;

        internal void AssignContext(IContext context)
        {
            _context = context;
        }
    }
}
