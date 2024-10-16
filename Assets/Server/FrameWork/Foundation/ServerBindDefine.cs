namespace Cr7Sund
{
    public static class ServerBindDefine
    {
        #region Global(game)
        public const string GameEventBus = "GameEventBus";
        #endregion

        #region 
        public const long SceneTimeOutTime = 2000;
        #endregion

        #region  UI
        public static IAssetKey UIRootAssetKey = new AssetKey("UIRoot");
        public const string UI_ROOT_NAME = "UIROOT";
        public const long UITimeOutTime = 2000;

        #endregion

        #region  Timer

        public const string UITimer = "UITimer";
        public const string SceneTimer = "SceneTimer";
        public const string GameTimer = "GameTimer";

        #endregion

        #region Logger

        public const string UILogger = "UILogger";
        public const string SceneLogger = "SceneLogger";
        public const string GameLogger = "GameLogger";

        #endregion

        #region  InstanceContainer
        /// <summary>
        /// don destroy on load
        /// </summary>
        public const string GameInstancePool = "GameInstancePool";
        public const string UIPanelUniqueContainer = "UIPanelUniqueContainer";
        public const string UIPanelContainer = "UIPanelContainer";

        #endregion
    }
}