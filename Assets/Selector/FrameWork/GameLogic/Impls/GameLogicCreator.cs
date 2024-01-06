using System;
using Cr7Sund.Selector.Apis;

namespace Cr7Sund.Selector.Impl
{
    public partial class GameLogicCreator
    {
        public Type type;

        internal static IGameLogic Create()
        {
            var creator = new GameLogicCreator();
            return creator.CreateGameLogic();
        }

        public IGameLogic CreateGameLogic()
        {
            return Activator.CreateInstance(type) as IGameLogic;
        }
    }


}