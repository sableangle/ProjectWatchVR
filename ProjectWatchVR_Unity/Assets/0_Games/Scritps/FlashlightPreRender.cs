using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightPreRender : MonoBehaviour
{

    Transform transformCache;
    public Transform RenderPlane;
    void Start()
    {
        RenderPlane.localScale = new Vector3(Screen.width / 100, Screen.height / 100, 1);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
