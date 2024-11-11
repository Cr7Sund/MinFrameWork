using Cr7Sund.IocContainer;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public interface INodeContext : IContext
    {
        PromiseTask AddComponents(INode node, IRouteArgs fragmentContext);
        void RemoveComponents();
    }
}
