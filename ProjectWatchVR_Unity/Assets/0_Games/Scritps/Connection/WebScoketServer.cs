using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebScoketServer : MonoBehaviour {
	public static WebScoketServer Instance;
	WebSocketServer wssv;
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
		wssv = new WebSocketServer (24681);
		wssv.AddWebSocketService<ConnectModels.WatchRotation> ("/Rotation");
		wssv.Start ();
	}

	public void SendMsg (string msg) {
		Instance.wssv.WebSocketServices.Broadcast (msg);
	}
	void OnApplicationQuit () {
		Instance.wssv.Stop ();
	}

}