using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationArmController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
		var r = new Vector3(InputModels.rotation.eulerAngles.y,-InputModels.rotation.eulerAngles.z,InputModels.rotation.eulerAngles.x);
		transform.eulerAngles = r;
	}
}
