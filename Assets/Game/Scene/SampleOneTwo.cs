using UnityEngine;

namespace Cr7Sund.Game.Scene
{
    public class SampleOneTwo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Info("Awake SampleOneTwo");
        }

        void OnEnable()
        {
            Debug.Info("OnEnable SampleOneTwo");
        }

        void OnDisable()
        {
            Debug.Info("OnDisable SampleOneTwo");
        }
        void OnDestroy()
        {
            Debug.Info("Destroy SampleOneTwo");
        }
    }
}
