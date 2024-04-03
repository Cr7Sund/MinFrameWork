using Cr7Sund.AssetLoader.Api;
using UnityEngine;

namespace Cr7Sund.Server.Impl
{
    public class GameInstanceContainer : BaseInstancesContainer
    {
        // public const string DontDestroyDefine = "DontDestroyOnLoad";
        [Inject(ServerBindDefine.GameLoader)] IAssetLoader _assetLoader;

        protected override IAssetLoader Loader => _assetLoader;


        protected override void MoveInstanceToScene(GameObject instance)
        {
            GameObject.DontDestroyOnLoad(instance);
        }
    }
}