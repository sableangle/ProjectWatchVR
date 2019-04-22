using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
	cakeslice.Outline outline;
	void Awake(){
		outline = gameObject.AddComponent<cakeslice.Outline>();
		outline.eraseRenderer = true;
	}
    public void OnPointEnter()
    {
		outline.eraseRenderer = false;
    }

    public void OnPointOut()
    {
		outline.eraseRenderer = true;
    }

    public void OnPick()
    {

    }
}
