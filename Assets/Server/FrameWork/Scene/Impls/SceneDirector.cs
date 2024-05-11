

using System.Threading;

namespace Cr7Sund.Server.Scene.Impl
{
    public static class SceneDirector
    {
        public static void Construct(SceneBuilder builder, SceneKey key)
        {
            builder.BuildContext();
            builder.BuildControllers();
            builder.BuildNode(key);
        }
    }
}