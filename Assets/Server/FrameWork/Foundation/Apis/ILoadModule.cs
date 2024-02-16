using Cr7Sund.Framework.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.Api
{
    public interface ILoadModule
    {
        IPromise<INode> RemoveNode(IAssetKey uiKey);
        IPromise<INode> AddNode(IAssetKey openUiKey);
    }
}
