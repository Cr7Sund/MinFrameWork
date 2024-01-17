using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleOneScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awake SampleOneScene");
    }

    void OnDestroy()
    {
        Debug.Log("Destroy SampleOneScene");
    }
}
