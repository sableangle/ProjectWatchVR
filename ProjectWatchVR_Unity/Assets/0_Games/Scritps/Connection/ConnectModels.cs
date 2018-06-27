using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ConnectModels : MonoBehaviour {
	public static Quaternion rotation;
	public class WatchRotation : WebSocketBehavior {
		protected override void OnMessage (MessageEventArgs e) {
			//Debug.Log("OnMessage"+e.Data);
			var msg = e.Data.Split('_');
			float w,x,y,z;
			float.TryParse(msg[0],out x);
			float.TryParse(msg[1],out y);
			float.TryParse(msg[2],out z);
			float.TryParse(msg[3],out w);
			rotation = new Quaternion(x,y,z,w);
		}
	}
}