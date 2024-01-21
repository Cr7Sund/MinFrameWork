using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface INode : ILifeTime, ILoadAsync<INode>, IInjectable, IRunnable, IInitialize, ILoading
    {
        INode Parent { get; }
        NodeState NodeState { get; }
        IAssetKey Key { get; }

        IPromise<INode> PreLoad(INode self);
        IPromise<INode> AddChildAsync(INode child);
        IPromise<INode> UnloadChildAsync(INode child);
        IPromise<INode> RemoveChildAsync(INode child);

    }


}
