using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.Game.Scene
{
    public class EditorSceneOneBuilder : SceneBuilder
    {
        protected override void AddControllers(IControllerModule controllerModule)
        {
            controllerModule.AddController<EditorSceneOneController>();
        }
        protected override SceneContext CreateContext()
        {
            return new EditorSceneOneContext();
        }
    }
}