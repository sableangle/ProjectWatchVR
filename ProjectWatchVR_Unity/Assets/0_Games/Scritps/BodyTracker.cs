using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTracker : MonoBehaviour
{
    public Transform targetObject;
    public Vector3 oriPosition;
    // Use this for initialization
    void Start()
    {
        oriPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, targetObject.eulerAngles.y, 0);
        transform.position = oriPosition + targetObject.localPosition;
    }
}
