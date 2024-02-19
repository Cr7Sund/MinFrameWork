

namespace Cr7Sund.Server.Scene.Impl
{
    public static class SceneDirector
    {
        public static void Construct(SceneBuilder builder)
        {
            builder.BuildContext();
            builder.BuildControllers();
            builder.BuildNode();
        }
    }
}