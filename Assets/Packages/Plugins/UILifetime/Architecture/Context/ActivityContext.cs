using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
namespace Cr7Sund.LifeTime
{
    public abstract class ActivityContext : RouteContext, INodeContext
    {
        protected sealed override string Channel
        {
            get => ServerBindDefine.ActivityLogger;
        }
        
        public ActivityContext() : base(string.Empty)
        {

        }

        public async override PromiseTask AddComponents(INode node, IRouteArgs fragmentContext)
        {
            await base.AddComponents(node, fragmentContext);
            var instanceContainer = ContainerFactory.Create(_sceneName);
            var uniqueInstanceContainer = ContainerFactory.Create(_sceneName);

            BindAsCrossAndSingleton<IPromiseTimer, PromiseTimer>();
            BindInstanceAsCrossContext<IInstancesContainer>(instanceContainer, ServerBindDefine.ActivityContainer);
            BindInstanceAsCrossContext<IUniqueInstanceContainer>(uniqueInstanceContainer, ServerBindDefine.ActivityUniqueContainer);
        }

        public override void RemoveComponents()
        {
            base.RemoveComponents();
            Unbind<IPromiseTimer>();
            Unbind<IInstancesContainer>(ServerBindDefine.ActivityContainer);
            Unbind<IUniqueInstanceContainer>(ServerBindDefine.ActivityUniqueContainer);
        }
    }
}
