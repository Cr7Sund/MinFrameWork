

namespace Cr7Sund.Server.Scene.Impl
{
    public static class SceneDirector
    {
        public static async PromiseTask Construct(SceneBuilder builder, SceneKey key)
        {
            builder.BuildContext();
            await builder.BuildControllers();
            builder.BuildNode(key);
        }
    }
}