using System;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Navigation
{
    public static class TransitionRouteFactory
    {

        public static INavGraph Create<TInstance>()where TInstance : INavGraph, new()
        {
            return Activator.CreateInstance<TInstance>();
        }
    }


}