using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AssetLoaderFactory
    {
        public static IAssetLoader CreateLoader()
        {
            if (true)
            {
                return new AddressableLoader();
            }
        }

        public static ISceneLoader CreateSceneLoader()
        {
            if (true)
            {
                return new AddressableSceneLoader();
            }
        }
    }
}
