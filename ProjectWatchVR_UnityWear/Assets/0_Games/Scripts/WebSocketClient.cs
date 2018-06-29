using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class WebSocketClient : MonoBehaviour {
	string msg = null;
	string ip = "192.168.0.131";
	WebSocket ws;
	public static WebSocketClient Instance;
	void Awake(){
		Instance = this;
		Connect();
	}
	void Connect () {
		ws = new WebSocket ("ws://" + ip + ":24681/Rotation");

		ws.OnError += (sender, e) => {
			Debug.LogException (e.Exception);
		};

		ws.OnMessage += (sender, e) => {
			msg = e.Data;

			Debug.Log ("Says: " + e.Data);
		};

		ws.OnClose += (sender, e) => {
			Debug.Log ("Closed");
		};

		ws.OnOpen += (sender, e) => {
			//ws.Send("Hello");
			//ws.Send("Siyuan");
			//ws.Send("Wang");
		};

		ws.Connect ();
	}
	public void SendMessageToServer (string msg) {
		if(!ws.IsAlive){
			Debug.LogError("Server is not running");
			return;
		}
		ws.Send(msg);
	}

}