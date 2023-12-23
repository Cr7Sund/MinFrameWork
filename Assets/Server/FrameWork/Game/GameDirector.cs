
namespace Cr7Sund.Server.Impl
{
    public static class GameDirector
    {
        public static void Construct(GameBuilder builder)
        {
            builder.BuildContext();
            builder.BuildNode();
            builder.BuildControllers();
        }
    }
}