

namespace Cr7Sund.Server.Impl
{
    public static class SceneDirector
    {
        public static void Construct(SceneBuilder builder)
        {
            builder.BuildNode();
            builder.BuildContext();
            builder.BuildControllers();
        }
    }
}