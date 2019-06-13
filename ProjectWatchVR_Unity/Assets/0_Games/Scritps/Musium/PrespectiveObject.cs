using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrespectiveObject : MonoBehaviour, IPickable
{
    public Color outlineColor = Color.green;
    public Transform GetTransform()
    {
        return transform;
    }
    public MonoBehaviour GetBehaviour()
    {
        return this;
    }

    public void OnPickFinish()
    {

    }

    public void OnPickStart(Transform pickPointer)
    {

    }
    static string _OutlineWidth = "_OutlineWidth";
    public void OnPointEnter()
    {
        renderer.material.SetFloat(_OutlineWidth, 0.02f);
    }
    public void OnPointOut()
    {
        renderer.material.SetFloat(_OutlineWidth, 0.0f);
    }


    Renderer _renderer;

    Renderer renderer
    {
        get
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            return _renderer;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        renderer.material.SetColor("_OutlineColor", outlineColor);

    }

    void Awake()
    {
        renderer.material.SetFloat(_OutlineWidth, 0.0f);
    }



}
