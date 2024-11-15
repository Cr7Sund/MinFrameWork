using Cr7Sund.PackageTest.IOC;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.AssetContainers;

namespace Cr7Sund.Server.Test
{
    public class SampleGameBuilder : GameBuilder
    {
        protected override GameContext CreateContext()
        {
            return new SampleGameContext();
        }

        protected override BaseGameController CreateController()
        {
            return new SampleGameController();
        }

        protected override GameNode CreateGameNode()
        {
            return new GameNode();
        }
    }
}