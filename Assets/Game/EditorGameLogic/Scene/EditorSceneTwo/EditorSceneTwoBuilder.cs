using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.Game.Scene
{
    public class EditorSceneTwoBuilder : SceneBuilder
    {
        protected override async PromiseTask AddControllers(IControllerModule controllerModule)
        {
            await controllerModule.AddController<EditorSceneTwoController>();
        }
        protected override SceneContext CreateContext()
        {
            return new EditorSceneTwoContext();
        }
    }
}