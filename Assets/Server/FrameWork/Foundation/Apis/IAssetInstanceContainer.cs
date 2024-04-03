using Cr7Sund.AssetLoader.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IAssetInstanceContainer : IAssetContainer
    {
        IAssetPromise CreateInstance(IAssetKey key) ;
        IAssetPromise CreateInstanceAsync(IAssetKey key) ;

    }
}
