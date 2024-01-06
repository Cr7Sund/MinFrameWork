using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Server.Impl
{
    public class SceneNode : ModuleNode
    {
        public SceneKey Key { get; set; }


        internal void AssignContext(IContext context)
        {
            _context = context;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _context.InjectionBinder.Bind<SceneNode>().To(this);
        }
    }
}
