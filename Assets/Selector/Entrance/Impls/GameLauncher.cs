using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.GameLogic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Selector.Apis;
using UnityEngine;
using Cr7Sund.Config;
using Cr7Sund.Server.Utils;

namespace Cr7Sund.Selector.Impl
{
    public class GameLauncher : MonoBehaviour
    {
        private IGameLogic _gameLogic;
        private TimeCorrector _updateCorrector;
        private TimeCorrector _lateUpdateCorrector;
        private bool _dispose;



        internal IPromise<INode> Dispose()
        {
            if (!_dispose)
            {
                _dispose = true;
                return _gameLogic?.Destroy();
            }

            return Promise<INode>.Resolved(null);
        }

        private void Awake()
        {
            _gameLogic = CreateGameLogic();
            _updateCorrector = new TimeCorrector();
            _lateUpdateCorrector = new TimeCorrector();
        }
        private void Start()
        {
            _gameLogic.Start();
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
        }

        private IGameLogic CreateGameLogic()
        {
            using (var configLoader = AssetLoaderFactory.CreateLoader())
            {
                configLoader.Init();
                var gameConfig = configLoader.LoadSync<GameConfig>(ConfigDefines.GameConfig);
                var gameLogic = gameConfig.GetLogic();
                gameLogic.Init();
                return gameLogic;
            }

        }
    }
}