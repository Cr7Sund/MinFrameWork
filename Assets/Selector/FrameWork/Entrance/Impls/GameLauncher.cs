using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Selector.Apis;
using UnityEngine;

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
                return _gameLogic.Destroy();
            }

            return Promise<INode>.Resolved(null);
        }

        private void Awake()
        {
            _gameLogic = GameLogicCreator.Create();
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

    }
}