

namespace Cr7Sund.LifeTime
{
    public class UIConfigDefines
    {
        #region  InstanceContainer
        /// <summary>
        /// don destroy on load
        /// </summary>
        public const string GameInstancePool = "GameInstancePool";
        public const string UIPanelUniqueContainer = "UIPanelUniqueContainer";
        public const string UIPanelContainer = "UIPanelContainer";

        #endregion
        
        public static IAssetKey UIRootAssetKey = new AssetKey("UIRoot");
        public const string UI_ROOT_NAME = "UIROOT";
        public readonly static AssetKey UITransitionConfig = new AssetKey("Assets/Preresources/Config/UI/UITransitionConfig.asset");
    }
}
