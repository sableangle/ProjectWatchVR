using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        if (isGround)
        {
            //Do something
        }
        else
        {
            //Do something

            var r = GetComponentInChildren<Renderer>();


            foreach (var item in r.materials)
            {
                item.DOFloat(1, "_DissolveAmount", 0.4f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
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

    public void SetIsGround(bool isGround)
    {
        this.isGround = isGround;
        OnIsGroundChange();
    }

    void OnIsGroundChange()
    {

        var r = GetComponentInChildren<Renderer>();
        if (isGround)
        {
            foreach (var item in r.materials)
            {
                item.DOFade(1f, 0.4f);
            }
        }
        else
        {
            foreach (var item in r.materials)
            {
                item.DOFade(0.25f, 0.4f);
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _transform = transform;
        OnIsGroundChange();
    }

    void Update()
    {
        if (isPick)
        {
            _transform.position = WearController.Instance.pointer.position;

        }

    }
}
