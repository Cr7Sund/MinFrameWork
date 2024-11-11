using System.Collections.Generic;
using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public class NavHost : BaseLifeCycleAwareComponent
    {
        private readonly Dictionary<INavGraph, NavController> _containers = new();


        public NavController FindNavController(IRouteKey id)
        {
            if (id is not INavGraph navGraph)
            {
                navGraph = id.ParentKey as INavGraph;
            }

            if (_containers.TryGetValue(navGraph, out var navController))
            {
                navController = AddController(navGraph);
            }

            return navController;
        }

        private NavController AddController(INavGraph navGraph)
        {
            var navController = NavController.CreateNavController();
            _containers.Add(navGraph, navController);
            return navController;
        }

    }
}
