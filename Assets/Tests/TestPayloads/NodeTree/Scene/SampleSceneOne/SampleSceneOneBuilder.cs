using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Framework.Tests
{
    public class SampleSceneOneBuilder : SceneBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<SampleSceneOneController>();
        }
        protected override SceneContext CreateContext()
        {
            return new SampleSceneOneContext();
        }
    }
}