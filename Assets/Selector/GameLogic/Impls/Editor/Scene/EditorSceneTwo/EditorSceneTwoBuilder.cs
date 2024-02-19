using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class EditorSceneTwoBuilder : SceneBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<EditorSceneTwoController>();
        }
        protected override SceneContext CreateContext()
        {
            return new EditorSceneTwoContext();
        }
    }
}