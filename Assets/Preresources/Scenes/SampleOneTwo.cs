using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleOneTwo : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awake SampleOneTwo");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable SampleOneTwo");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable SampleOneTwo");
    }
    void OnDestroy()
    {
        Debug.Log("Destroy SampleOneTwo");
    }
}
