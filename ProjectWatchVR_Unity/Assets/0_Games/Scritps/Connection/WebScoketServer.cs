using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebScoketServer : MonoBehaviour {
	public static WebScoketServer Instance;
	WebSocketServer webSocketServer;
	WebSocketServer intpuSocket;
	void Awake () {
		DontDestroyOnLoad (gameObject);

		if (Instance == null) {
			Instance = this;
			Init ();
		} else {
			return;
		}
	}
	// Use this for initialization
	void Init () {
		webSocketServer = new WebSocketServer (24681);
		webSocketServer.AddWebSocketService<InputModels.WatchSensor> ("/Sensor");
		webSocketServer.AddWebSocketService<InputModels.WatchInput> ("/Input");
		webSocketServer.Start ();
	}

	public void SendMsg (string msg) {
		Instance.webSocketServer.WebSocketServices.Broadcast (msg);
	}
	void OnApplicationQuit () {
		Instance.webSocketServer.Stop ();
	}

}