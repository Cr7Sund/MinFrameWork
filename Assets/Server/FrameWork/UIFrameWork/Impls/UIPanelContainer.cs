using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using UnityEngine;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIPanelContainer : BaseInstancesContainer, IInstancesContainer
    {
        [Inject] private IAssetLoader _assetLoader;
        protected override IAssetLoader Loader => _assetLoader;


        public override void Dispose()
        {
            base.Dispose();
        }
        protected override void OnCreate(GameObject instance)
        {
        }

    }

    public class UIPanelUniqueContainer : BaseUniqueInstanceContainer, IUniqueInstanceContainer
    {
        [Inject] private IAssetLoader _assetLoader;
        protected override IAssetLoader Loader => _assetLoader;
    }
}