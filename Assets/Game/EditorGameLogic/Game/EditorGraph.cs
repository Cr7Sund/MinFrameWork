using System.Collections.Generic;
using Cr7Sund.Game.Scene;
using Cr7Sund.Game.UI;
using Cr7Sund.LifeTime;
namespace Cr7Sund.Game.GameLogic
{
    public static class EditorGraph
    {
        public static readonly IRouteKey RootKey = new RouteKey("RootActivity",
            typeof(EditorGameActivity),
            typeof(EditorGameContext)
        );
        
        public static readonly IRouteKey LoginGraphKey = new GraphKey("LoginGraph", new IRouteKey[]
        {
            EditorSceneKeys.EditorAddictiveSceneKeyOne,
            EditorUIKeys.SampleOneUI
        });
        
        public static readonly IRouteKey MainGameGraphKey = new GraphKey("MainGameGraph", new IRouteKey[]
        {
            EditorSceneKeys.EditorAddictiveSceneKeyTwo,
            EditorUIKeys.SampleThreeUI
        });
    }
}
