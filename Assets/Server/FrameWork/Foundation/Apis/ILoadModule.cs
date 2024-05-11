using System;

namespace Cr7Sund.Server.Api
{
    public interface ILoadModule : IDisposable
    {
        //  remove a node with the specified asset key
        // Optional parameter 'overwrite' to indicate whether the existed operation should be overwritten
        PromiseTask RemoveNode(IAssetKey assetKey, bool overwrite = false);
        
        //  unload a node with the specified asset key
        // Optional parameter 'overwrite' to indicate whether the existed operation should be overwritten
        PromiseTask UnloadNode(IAssetKey assetKey, bool overwrite = false);

        // dd a node with the specified asset key
        // Optional parameter 'overwrite' to indicate whether the existed operation should be overwritten
        PromiseTask AddNode(IAssetKey assetKey, bool overwrite = false);
    }
}
