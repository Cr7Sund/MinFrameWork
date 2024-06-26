using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public class ConfigContainer : BaseAssetContainer, IConfigContainer
    {
        [Inject] private IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;


        public async PromiseTask<T> GetConfig<T>(IAssetKey assetKey) where T : Object
        {
            return await LoadAsset<T>(assetKey);
        }

        public async void RemoveConfigAsync(IAssetKey assetKey)
        {
            await base.Unload(assetKey);
        }
    }
}