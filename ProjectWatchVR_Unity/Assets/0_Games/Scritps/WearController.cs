using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour
{
    public Material screenMaterial;
    public bool editorSimlator = false;
    void Start()
    {
    }
    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;
    private const string AXIS_MOUSE_X = "Mouse X";
    private const string AXIS_MOUSE_Y = "Mouse Y";
    public Quaternion WearRotation { get; private set; }

    void Update()
    {
        if (!editorSimlator)
        {
            var r = new Vector3(0, ConnectModels.rotation.eulerAngles.z, -ConnectModels.rotation.eulerAngles.x);
            transform.eulerAngles = r;
            SetTouchPosition(ConnectModels.screenPosition);
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                mouseX += Input.GetAxis(AXIS_MOUSE_X) * 6;
                if (mouseX <= -180)
                {
                    mouseX += 360;
                }
                else if (mouseX > 180)
                {
                    mouseX -= 360;
                }

                mouseY -= Input.GetAxis(AXIS_MOUSE_Y) * 6f;
                mouseY = Mathf.Clamp(mouseY, -85, 85);

                WearRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);

                // Update all VR cameras using Head position and rotation information.
                transform.localRotation = WearRotation;
            }
        }
    }

    void SetTouchPosition(Vector2 screenPosition)
    {
        // touchPosition.localPosition = new Vector3(screenPosition.x * lineUnit, screenPosition.y * lineUnit, 0.00378f);
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1 - screenPosition.y) * -10 + 0.5f));
    }

}
