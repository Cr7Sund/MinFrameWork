using Cr7Sund.Package.Api;  // Importing the framework's API namespace
using Cr7Sund.NodeTree.Api;  // Importing the node tree API namespace

namespace Cr7Sund.Server.Api
{
    public interface ILoadModule
    {
        //  remove a node with the specified asset key
        IPromise<INode> RemoveNode(IAssetKey assetKey);

        // dd a node with the specified asset key
        // Optional parameter 'overwrite' to indicate whether existing nodes should be overwritten
        IPromise<INode> AddNode(IAssetKey assetKey, bool overwrite = false);
    }
}
