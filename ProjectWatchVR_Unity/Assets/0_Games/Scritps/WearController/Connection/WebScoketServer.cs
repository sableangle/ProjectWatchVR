using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebScoketServer : MonoBehaviour
{
    public static WebScoketServer Instance;
    WebSocketServer webSocketServer;
    WebSocketServer intpuSocket;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            return;
        }
    }
    // Use this for initialization
    void Init()
    {
		Debug.Log("Start WebSocket");
        webSocketServer = new WebSocketServer(24681);
        webSocketServer.AddWebSocketService<VRInputReciver.WatchSensor>("/Sensor");
        webSocketServer.AddWebSocketService<VRInputReciver.WatchInput>("/Input");
        try
        {
            webSocketServer.Start();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void SendMsg(string msg)
    {
        Instance.webSocketServer.WebSocketServices.Broadcast(msg);
    }
    void OnApplicationQuit()
    {
        Instance.webSocketServer.Stop();
    }

}