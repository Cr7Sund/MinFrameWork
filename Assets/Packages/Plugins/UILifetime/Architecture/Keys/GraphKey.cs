using System;
using System.Collections.Generic;
using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public class GraphKey : RouteKey, INavGraph
    {
        public IEnumerable<IRouteKey> RouteKeys
        {
            get;
        }

        public GraphKey(string id, IEnumerable<IRouteKey> routeKeys) : base(id, null, null)
        {
            RouteKeys = routeKeys;
        }

        // public GraphKey(string id, Type fragmentType, Type contextType) : base(id, fragmentType, contextType)
        // {
        // }


    }
}
