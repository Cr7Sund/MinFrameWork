using Cr7Sund.PackageTest.IOC;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Tests
{
    public class SampleGameBuilder : GameBuilder
    {
        protected override GameContext CreateContext()
        {
            return new SampleGameContext();
        }

        protected override BaseGameController CreateController()
        {
            return new SampleController();
        }

        protected override GameNode CreateGameNode()
        {
            return new GameNode();
        }
    }
}