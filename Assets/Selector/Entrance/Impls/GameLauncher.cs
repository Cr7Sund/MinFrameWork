using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Selector.Apis;
using UnityEngine;
using Cr7Sund.Config;
using Cr7Sund.AssetLoader;

namespace Cr7Sund.Selector.Impl
{
    public class GameLauncher : MonoBehaviour
    {
        private IGameLogic _gameLogic;
        private TimeCorrector _updateCorrector;
        private TimeCorrector _lateUpdateCorrector;
        private bool _dispose;


        #region  Unity LifeCycles

        private void Awake()
        {
            InitFrameWork();

        }
        private void Start()
        {
            _gameLogic.Start();
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }


        private void Update()
        {
            _gameLogic.Update(_updateCorrector.GetCorrectedTime());
        }

        private void LateUpdate()
        {
            _gameLogic.LateUpdate(_lateUpdateCorrector.GetCorrectedTime());
        }

        private void OnApplicationQuit()
        {
            Dispose();
            EntranceConsole.Dispose();

            Console.Info("Exit Game!!!");
        }


        #endregion

        #region Custom Launcher

        private void InitFrameWork()
        {
            Console.Init(InternalLoggerFactory.Create("FrameWork"));

            _gameLogic = CreateGameLogic();
            _updateCorrector = new TimeCorrector();
            _lateUpdateCorrector = new TimeCorrector();
        }

        private IGameLogic CreateGameLogic()
        {
            var configLoader = AssetLoaderFactory.CreateLoader();
            var gameConfig = configLoader.LoadSync<GameConfig>(ConfigDefines.GameConfig);
            var gameLogic = gameConfig.CreateLogic();
            return gameLogic;
        }

        internal IPromise<INode> Dispose()
        {
            if (!_dispose)
            {
                _dispose = true;
                return _gameLogic?.Destroy();
            }

            Console.Dispose();
            return Promise<INode>.Resolved(null);
        }
        #endregion
    }
}