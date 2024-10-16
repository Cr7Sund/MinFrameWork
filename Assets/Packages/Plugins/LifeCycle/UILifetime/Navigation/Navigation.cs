using System;
using System.Collections.Generic;
using Cr7Sund.UILifeTime;
namespace Cr7Sund.Navigation
{
    public class Navigation
    {
        private readonly Dictionary<Guid, NavController> _containers = new();


        public NavController FindNavController(Guid id)
        {
            return _containers[id];
        }

        internal NavController CreateNavController(Activity activity)
        {
            var navController = new NavController(activity);
            _containers.Add(activity.ID, navController);
            return navController;
        }

        internal NavController CreateNavController(Fragment fragment)
        {
            var hostActivity = fragment.HostActivity;
            var navController = new NavController(hostActivity, fragment);
            _containers.Add(fragment.ID, navController);
            return navController;
        }

    }
}
