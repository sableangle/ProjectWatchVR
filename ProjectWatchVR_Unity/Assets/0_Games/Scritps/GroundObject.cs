using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundObject : MonoBehaviour, IPickable
{
    Transform _transform;
    public Transform GetTransform()
    {
        return transform;
    }

    public void OnPickFinish()
    {
        isPick = false;
    }

    bool isPick = false;
    public void OnPickStart(Transform pickPointer)
    {
        isPick = true;
    }

    public void OnPointEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointOut()
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        if (isPick)
        {
            _transform.position = WearController.Instance.pointer.position;
        }
    }
}
