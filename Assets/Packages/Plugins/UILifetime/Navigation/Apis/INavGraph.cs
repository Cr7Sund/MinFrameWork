using System.Collections.Generic;
namespace Cr7Sund.LifeTime
{
    public interface INavGraph
    {
        IEnumerable<IRouteKey> RouteKeys { get; }
    }

}
