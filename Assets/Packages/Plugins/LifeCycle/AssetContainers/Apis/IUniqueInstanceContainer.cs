using System.Threading;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetContainers
{
    public interface IUniqueInstanceContainer : IAssetContainer
    {
        PromiseTask<T> CreateInstance<T>(IAssetKey key) where T : Object;
        PromiseTask<T> CreateInstanceAsync<T>(IAssetKey key, UnsafeCancellationToken cancellation) where T : Object;
    }
}
