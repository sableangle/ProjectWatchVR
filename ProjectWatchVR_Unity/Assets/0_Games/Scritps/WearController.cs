using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour
{
    public Material screenMaterial;

    void Start()
    {
    }
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        var r = new Vector3(0,ConnectModels.rotation.eulerAngles.z,-ConnectModels.rotation.eulerAngles.x);
		transform.eulerAngles = r;
        SetTouchPosition(ConnectModels.screenPosition);
    }

    void SetTouchPosition(Vector2 screenPosition)
    {
        // touchPosition.localPosition = new Vector3(screenPosition.x * lineUnit, screenPosition.y * lineUnit, 0.00378f);
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1-screenPosition.y) * -10 + 0.5f));
    }

}
