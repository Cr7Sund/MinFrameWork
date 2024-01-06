using UnityEngine;

namespace Cr7Sund.Framework.Impl
{
    public class TimeCorrector
    {
        private float _addedLeftTime;
        private float _lastTime;


        public TimeCorrector()
        {
            _addedLeftTime = 0;
            _lastTime = Time.time;
        }


        public int GetCorrectedTime()
        {
            float current = Time.time;
            float deltaTime = current - _lastTime;
            _lastTime = current;

            float updateTime = deltaTime;
            int elapsedMilliseconds = Mathf.FloorToInt(updateTime * 1000);

            float leftTime = updateTime - elapsedMilliseconds / 1000.0f;
            _addedLeftTime += leftTime;

            if (_addedLeftTime >= 0.001f)
            {
                elapsedMilliseconds += 1;
                _addedLeftTime -= 0.001f;
            }

            return elapsedMilliseconds;
        }

    }
}