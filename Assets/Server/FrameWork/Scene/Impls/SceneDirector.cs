

namespace Cr7Sund.Server.Impl
{
    public static class SceneDirector
    {
        public static void Construct(SceneBuilder builder)
        {
            builder.BuildContext();
            builder.BuildNode();
        }
    }
}