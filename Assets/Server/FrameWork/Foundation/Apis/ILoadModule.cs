using Cr7Sund.Package.Api;  
using Cr7Sund.NodeTree.Api;  

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
