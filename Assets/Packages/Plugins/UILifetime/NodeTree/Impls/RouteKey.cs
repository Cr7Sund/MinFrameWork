using System;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public class RouteKey : IRouteKey
    {
        public Type FragmentKey { get; private set; }
        public Type ContextKey { get; protected set; }
        public string Key { get; private set; }
        public int Priority
        {
            get;
            set;
        }


        public IRouteKey ParentKey { get; set; }

        public bool OverwriteTask { get;  set; }

        public bool IsInStack { get; set; }

        public bool SkipHideAnimation { get; set; }
        public bool ParallelLoad
        {
            get;
            set;
        }
        public bool ParallelTransition
        {
            get;
            set;
        }



        public INode CreateNode()
        {
            var instance = Activator.CreateInstance(FragmentKey) as INode;
            instance.Init(this);
            return instance;
        }
        
        public INodeContext CreateContext()
        {
            if (ContextKey == null)
            {
                return null;
            }
            return Activator.CreateInstance(ContextKey) as INodeContext;
        }

        public RouteKey(string id, Type fragmentType, Type contextType = null)
        {
            Key = id;
            FragmentKey = fragmentType;
            ContextKey = contextType;
        }

        
    }

}
