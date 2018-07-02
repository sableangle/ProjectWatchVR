using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.rotation = ConnectModels.rotation;
	}
}
