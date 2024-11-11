using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneOneBuilder : SceneBuilder
    {
        protected override async PromiseTask AddControllers(IControllerModule controllerModule)
        {
            await controllerModule.AddController<SampleSceneOneController>();
        }
        protected override SceneContext CreateContext()
        {
            return new SampleSceneOneContext();
        }
    }
}