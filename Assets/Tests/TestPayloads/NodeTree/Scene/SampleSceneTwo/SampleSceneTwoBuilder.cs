using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Framework.Tests
{
    public class SampleSceneTwoBuilder : SceneBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<SampleSceneTwoController>();
        }
        protected override SceneContext CreateContext()
        {
            return new SampleSceneTwoContext();
        }
    }
}