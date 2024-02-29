using System.Collections;
using System.Collections.Generic;
using Cr7Sund.Logger;
using UnityEngine;

public class SampleServer : MonoBehaviour
{
    private UDPLogServer _server = null;
    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            _server = new UDPLogServer();
            _server.InitUDPClient();

            _server.RunReceiveTask();
        }
    }

    public void OnDestroy()
    {
        _server?.Dispose();
    }
}
