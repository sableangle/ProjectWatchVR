using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class SommonObject : MonoBehaviour, IPickable
{
    // cakeslice.Outline outline;
    //static Vector3 _gravity = new Vector3(0, -9.8f, 0);
    Rigidbody rigibody;
    Transform _transform;
    void Awake()
    {
        rigibody = GetComponent<Rigidbody>();
        _transform = transform;
        renderer.material.SetFloat(_OutlineWidth, 0.0f);
    }
    void Start()
    {
        renderer.material.SetColor("_OutlineColor", outlineColor);
    }

    cakeslice.Outline outline;
    Renderer _renderer;

    Renderer renderer
    {
        get
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            return _renderer;
        }
    }
    static string _OutlineWidth = "_OutlineWidth";

    float dragDamper = 15.0f;
    void FixedUpdate()
    {
        if (isPicked)
        {
            var velocity = picker.position - _transform.position;
            rigibody.velocity = velocity * dragDamper;
        }
    }
    public void OnPointEnter()
    {
        renderer.material.SetFloat(_OutlineWidth, 0.02f);
    }

    public void OnPointOut()
    {
        renderer.material.SetFloat(_OutlineWidth, 0.0f);
    }

    bool isPicked
    {
        get
        {
            return picker != null;
        }
    }
    private Transform picker;
    public void OnPickStart(Transform pickPointer)
    {
        picker = pickPointer;
        rigibody.useGravity = false;
        rigibody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public void OnPickFinish()
    {
        rigibody.useGravity = true;

        picker = null;
        rigibody.constraints = RigidbodyConstraints.None;
    }

    [SerializeField]
    private Color outlineColor = Color.yellow;
    public float LerpFactor = 10;

    public Transform GetTransform()
    {
        return transform;
    }
    public MonoBehaviour GetBehaviour()
    {
        return this;
    }
}
