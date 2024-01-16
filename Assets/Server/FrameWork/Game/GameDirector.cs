
using System;

namespace Cr7Sund.Server.Impl
{
    public static class GameDirector
    {
        public static GameNode Construct<T>() where T: GameBuilder, new()
        {
            var builder = Activator.CreateInstance<T>();
            return Construct(builder);
        }

        public static GameNode Construct(GameBuilder builder)
        {
            var gameNode = builder.BuildNode();
            builder.BuildContext();
            builder.BuildControllers();

            return gameNode;
        }
    }
}