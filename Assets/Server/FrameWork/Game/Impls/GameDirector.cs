﻿
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
            builder.BuildContext();
            builder.BuildControllers();

            var gameNode = builder.BuildNode();

            return gameNode;
        }
    }
}