
namespace Cr7Sund.Server.Impl
{
    public static class GameDirector
    {
        public static GameNode Construct(GameBuilder Server)
        {
            var gameNode = Server.BuildNode();
            Server.BuildContext();
            Server.BuildControllers();

            return gameNode;
        }
    }
}