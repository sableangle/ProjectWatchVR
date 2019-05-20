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


    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_OutlineColor", outlineColor);
        renderer.material.SetFloat(_OutlineWidth, 0.0f);

    }



}
