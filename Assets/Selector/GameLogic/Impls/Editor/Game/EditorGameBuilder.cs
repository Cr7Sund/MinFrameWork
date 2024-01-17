using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Selector.Impl
{
    public class EditorGameBuilder : GameBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<EditorMainController>();
            controllerModule.AddController<EditorAdditiveSceneController>();
        }

        protected override GameContext CreateContext()
        {
            return new EditorGameContext();
        }

    }
}