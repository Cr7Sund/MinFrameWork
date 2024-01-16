using System;
using Cr7Sund.Framework.Util;
using Cr7Sund.Selector.Apis;
using UnityEngine;

namespace Cr7Sund.Selector.Impl
{
    public partial class GameLogicCreator
    {
        public Type type;

        internal static IGameLogic Create()
        {
            IGameLogic gameLogic = null;
            if (MacroDefine.IsEditor)
            {
                gameLogic = new EditorGameLogic();
            }

            gameLogic.Init();

            return gameLogic;
        }


    }


}