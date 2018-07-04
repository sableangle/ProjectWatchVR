using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour {

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		var r = new Vector3(ConnectModels.rotation.eulerAngles.x,ConnectModels.rotation.eulerAngles.z,ConnectModels.accelerometer.x);
		transform.eulerAngles = r;
	}
}
