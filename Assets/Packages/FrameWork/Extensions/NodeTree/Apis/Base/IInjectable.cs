using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface IInjectable
    {
        bool IsInjected { get; }
        
        
        void Inject();
        void DeInject();
    }
}
