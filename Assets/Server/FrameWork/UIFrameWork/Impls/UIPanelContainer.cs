using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIPanelContainer : BaseUniqueInstanceContainer, IUniqueInstanceContainer
    {
        [Inject(ServerBindDefine.GameLoader)] private IAssetLoader _assetLoader;
        protected override IAssetLoader Loader => _assetLoader;
        
    }
}