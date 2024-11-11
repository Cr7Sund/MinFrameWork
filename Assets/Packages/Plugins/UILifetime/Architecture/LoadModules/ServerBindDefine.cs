namespace Cr7Sund
{
    public static class ServerBindDefine
    {
        #region Global(game)
        public const string GameEventBus = "GameEventBus";
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

        public const string GraphLogger = nameof(GraphLogger);
        public const string ActivityLogger = nameof(ActivityLogger);
        public const string GameLogger = nameof(GameLogger);

        #endregion

        #region  InstanceContainer
        /// <summary>
        /// don destroy on load
        /// </summary>
        public const string GameInstancePool = "GameInstancePool";
        public const string ActivityUniqueContainer = nameof(ActivityUniqueContainer);
        public const string ActivityContainer = nameof(ActivityContainer);

        public const string GameInstance = nameof(GameInstance);
        public const string GameUniqueInstance= nameof(GameUniqueInstance);
        #endregion
    }
}