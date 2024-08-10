using Cr7Sund.IocContainer;

namespace Cr7Sund.NodeTree.Api
{
    public interface INodeContext : IContext
    {
        void AddComponents(INode node);
        void RemoveComponents();
    }
}
