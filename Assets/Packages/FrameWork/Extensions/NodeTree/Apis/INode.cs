using System.Collections.Generic;
using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface INode : ILifeTime, ILoadAsync, IInjectable, IRunnable, IInitializable
    {
        INode Parent { get; }
        
        IPromise AddChildAsync(INode child);
        IPromise UnloadChildAsync(INode child);
    }

}
