using System;
using UnityEngine;

namespace Cr7Sund.Game.Scene
{
    public class SampleOneScene : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Info("Awake SampleOneScene");
        }

        private void OnEnable()
        {
        }

        void OnDestroy()
        {
        }
    }

}
