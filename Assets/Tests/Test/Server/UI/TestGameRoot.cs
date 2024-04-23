using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Selector.Apis;
using UnityEngine;

namespace Cr7Sund.ServerTest.UI
{
    public class TestGameRoot : MonoBehaviour
    {
        private IGameLogic _gameLogic;
        private TimeCorrector _updateCorrector;
        private TimeCorrector _lateUpdateCorrector;

        public IPromiseTimer PromiseTimer;
        public void Init(IGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
            PromiseTimer = new PromiseTimer();
        }

        private void Awake()
        {
            _updateCorrector = new TimeCorrector();
            _lateUpdateCorrector = new TimeCorrector();
        }

        private void Update()
        {
            if (_gameLogic != null)
            {
                int millisecond = _updateCorrector.GetCorrectedTime();
                _gameLogic?.Update(millisecond);
                PromiseTimer.Update(millisecond);
            }
        }

        private void LateUpdate()
        {
            if (_gameLogic != null)
            {
                _gameLogic?.LateUpdate(_lateUpdateCorrector.GetCorrectedTime());
            }

        }


    }
}
