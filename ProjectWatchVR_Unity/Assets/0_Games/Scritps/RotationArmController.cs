using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationArmController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
		var r = new Vector3(ConnectModels.rotation.eulerAngles.y,-ConnectModels.rotation.eulerAngles.z,ConnectModels.rotation.eulerAngles.x);
		transform.eulerAngles = r;
	}
}
