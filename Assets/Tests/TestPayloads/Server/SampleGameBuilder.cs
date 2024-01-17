using Cr7Sund.Framework.Tests;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Tests
{
    public class SampleGameBuilder : GameBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<SampleController>();
        }

        protected override GameContext CreateContext()
        {
            return new SampleGameContext();
        }

        protected override GameNode CreateGameNode()
        {
            return new GameNode();
        }
    }
}