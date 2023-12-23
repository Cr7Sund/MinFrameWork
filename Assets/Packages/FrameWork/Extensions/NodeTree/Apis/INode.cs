using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface INode : ILifeTime, ILoadAsync<INode>, IInjectable, IRunnable, IInitialize
    {
        INode Parent { get; }
        
        IPromise<INode> AddChildAsync(INode child);
        IPromise<INode> UnloadChildAsync(INode child);
    }

}
