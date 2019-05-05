using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundObject : MonoBehaviour, IPickable
{
    Transform _transform;
    public bool isGround = false;
    public Transform GetTransform()
    {
        return transform;
    }

    public void OnPickFinish()
    {
        isPick = false;

        if(isGround){
            //Do something
        }
        else{
            //Do something
            Destroy(gameObject);
        }
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

    public void SetIsGround(bool isGround){
        this.isGround = isGround;
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
