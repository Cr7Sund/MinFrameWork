using System;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Package.Impl;
using Cr7Sund.Selector.Apis;
using UnityEngine;
using Cr7Sund.Config;

namespace Cr7Sund.Selector.Impl
{
    public class GameLauncher : MonoBehaviour, IDestroyAsync
    {
        private IGameLogic _gameLogic;
        private TimeCorrector _updateCorrector;
        private TimeCorrector _lateUpdateCorrector;
        private bool _dispose;


        #region  Unity LifeCycles

        private async void Awake()
        {
            await InitFrameWork();
        }

        private async void Start()
        {
            await _gameLogic.Run();
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        private void Update()
        {
            _gameLogic?.Update(_updateCorrector.GetCorrectedTime());
        }

        private void LateUpdate()
        {
            _gameLogic?.LateUpdate(_lateUpdateCorrector.GetCorrectedTime());
        }

        private async void OnApplicationQuit()
        {
            await DestroyAsync();
            EntranceConsole.Dispose();

            Console.Info("Exit Game!!!");
        }

        #endregion

        #region Custom Launcher

        private async PromiseTask InitFrameWork()
        {
            Console.Init(InternalLoggerFactory.Create("FrameWork"));

            _gameLogic = await CreateGameLogic();
            _updateCorrector = new TimeCorrector();
            _lateUpdateCorrector = new TimeCorrector();
            _gameLogic.Init();
        }

        private async PromiseTask<GameLogic.GameLogic> CreateGameLogic()
        {
            var configLoader = AssetLoaderFactory.CreateLoader();
            var gameConfig = await configLoader.Load<GameConfig>(ConfigDefines.GameConfig);
            var gameLogic = gameConfig.CreateLogic();
            return gameLogic;
        }

        public async PromiseTask DestroyAsync()
        {
            if (!_dispose)
            {
                _dispose = true;
                if (_gameLogic != null)
                {
                    await _gameLogic.DestroyAsync();
                }
            }

            Console.Dispose();
        }
        #endregion
    }
}