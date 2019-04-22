using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    cakeslice.Outline outline;
    Rigidbody rigibody;
    void Awake()
    {
        outline = gameObject.AddComponent<cakeslice.Outline>();
        outline.eraseRenderer = true;

        rigibody = GetComponent<Rigidbody>();
    }
    public void OnPointEnter()
    {
        outline.eraseRenderer = false;
    }

    public void OnPointOut()
    {
        outline.eraseRenderer = true;
    }

    public void OnPickStart()
    {
        if (rigibody) rigibody.isKinematic = true;
    }
    public void OnPickFinish()
    {
        if (rigibody) rigibody.isKinematic = false;
    }
}
