using Object = UnityEngine.Object;

namespace Cr7Sund.LifeTime
{
    public interface IUniqueInstanceContainer : IAssetContainer
    {
        PromiseTask<T> Instantiate<T>(IAssetKey key) where T : Object;
        PromiseTask<T> InstantiateAsync<T>(IAssetKey key, UnsafeCancellationToken cancellation) where T : Object;
        public T GetInstance<T>(IAssetKey assetKey) where T : Object;
    }
}
