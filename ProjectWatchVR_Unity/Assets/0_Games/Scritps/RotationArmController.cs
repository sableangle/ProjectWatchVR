using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationArmController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
		var r = new Vector3(VRInput.rotation.eulerAngles.y,-VRInput.rotation.eulerAngles.z,VRInput.rotation.eulerAngles.x);
		transform.eulerAngles = r;
	}
}
