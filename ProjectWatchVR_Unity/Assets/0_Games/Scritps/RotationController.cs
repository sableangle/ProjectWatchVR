using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
		var r = new Vector3(ConnectModels.rotation.eulerAngles.x,ConnectModels.rotation.eulerAngles.y,-ConnectModels.rotation.eulerAngles.z);
		transform.rotation = ConnectModels.rotation;
	}
}
