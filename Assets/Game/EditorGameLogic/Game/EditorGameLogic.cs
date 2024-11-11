using Cr7Sund.Game.Scene;
using Cr7Sund.LifeTime;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorGameLogic : Cr7Sund.GameLogic.GameLogic
    {

        protected override IRouteKey routeKey
        {
            get => EditorGraph.RootKey;
        }
    }

}
