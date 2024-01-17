using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Framework.Tests
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