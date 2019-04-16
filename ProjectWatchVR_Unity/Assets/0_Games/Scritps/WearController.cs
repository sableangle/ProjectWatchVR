using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour
{
    public Material screenMaterial;

    void Start()
    {
        ConnectModels.OnWatchMoveChange += CalucLinePosition;
    }
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        var r = new Vector3(ConnectModels.rotation.eulerAngles.x, ConnectModels.rotation.eulerAngles.z, ConnectModels.accelerometer.x);
        transform.eulerAngles = r;
        SetTouchPosition(ConnectModels.screenPosition);
    }

    void SetTouchPosition(Vector2 screenPosition)
    {
        // touchPosition.localPosition = new Vector3(screenPosition.x * lineUnit, screenPosition.y * lineUnit, 0.00378f);
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1-screenPosition.y) * -10 + 0.5f));
    }

    public float lineUnit = 0.021f;
    public List<Vector3> linePositions = new List<Vector3>();
    Vector2 lastPosition;
    void CalucLinePosition(Vector2 screenPosition)
    {
        if (lastPosition == screenPosition)
        {
            return;
        }
    }
}
