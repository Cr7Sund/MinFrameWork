using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public class ConfigContainer : BaseAssetContainer, IConfigContainer
    {
        [Inject(ServerBindDefine.GameLoader)] private IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;


        public T GetConfig<T>(IAssetKey assetKey) where T : Object
        {
            return base.GetAssetSync<T>(assetKey);
        }

        public IAssetPromise GetConfigAsync(IAssetKey assetKey)
        {
            return base.GetAssetAsync(assetKey);
        }

    }
}