using System.Threading;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.AssetContainers;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneTwoBuilder : SceneBuilder
    {
        protected override async PromiseTask AddControllers(IControllerModule controllerModule)
        {
          await  controllerModule.AddController<SampleSceneTwoController>();
        }
        protected override SceneContext CreateContext()
        {
            return new SampleSceneTwoContext();
        }
    }
}