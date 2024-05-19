using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using UnityEngine;

namespace Cr7Sund
{

    public class EditorApplicationProxy 
    {
        private List<IUpdatable> _updates = new();

        public EditorApplicationProxy()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }

        public void Dispose()
        {
            AssertUtil.LessOrEqual(_updates.Count, 0);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
#endif
        }

#if UNITY_EDITOR
        // gc finalization queue
        ~EditorApplicationProxy()
        {
            UnityEditor.EditorApplication.update -= Update;
        }
#endif

        public void RegisterUpdateCallback(IUpdatable updatable)
        {
            _updates.Add(updatable);

        }

        public void UnRegisterUpdateCallback(IUpdatable updatable)
        {
            _updates.Remove(updatable);
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            for (int i = _updates.Count - 1; i >= 0; i--)
            {
                IUpdatable item = _updates[i];
                int millisecond = 200;
                item.Update(millisecond);
            }
        }
    }
}