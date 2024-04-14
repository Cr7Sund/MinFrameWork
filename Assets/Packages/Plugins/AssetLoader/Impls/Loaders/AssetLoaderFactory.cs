using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Impl;
using UnityEngine;

namespace Cr7Sund.AssetLoader.Impl
{
    public class AssetLoaderFactory
    {
        public static IAssetLoader CreateLoader()
        {
            if (MacroDefine.IsMainThread && Application.isPlaying)
            {
                var addressableLoader = new AddressableLoader();
                addressableLoader.Init();
                return addressableLoader;
            }
            else
            {
                return new EditorResourcesLoader();
            }
        }

        public static ISceneLoader CreateSceneLoader()
        {
            return new AddressableSceneLoader();
        }
    }
}
