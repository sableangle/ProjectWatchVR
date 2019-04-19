using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour
{
    private Transform transformCache;
    public Material screenMaterial;
    public bool editorSimlator = false;

    void Awake()
    {
        transformCache = transform;
    }
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
            // var r = new Vector3(InputModels.rotation.eulerAngles.y, InputModels.rotation.eulerAngles.z, 0);
            // transform.eulerAngles = r;
            transformCache.rotation = InputModels.rotation;
            transformCache.eulerAngles = new Vector3(transformCache.eulerAngles.x, transformCache.eulerAngles.y - transformCache.parent.eulerAngles.y, InputModels.accelerometer.x * 10);
            SetTouchPosition(InputModels.screenPosition);
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

                WearRotation = Quaternion.Euler(mouseY, mouseX - transformCache.parent.eulerAngles.y, mouseZ);

                // Update all VR cameras using Head position and rotation information.
                transformCache.localRotation = WearRotation;
            }
        }
        RayCast();
    }

    public Transform pointer;
    public Vector3 oriPointerPosition = new Vector3(0, 0, 1.6f);
    private bool isHit = false;
    private RaycastHit hit;
    public float lerpSpeed = 10;
    void RayCast()
    {
        Vector3 fwd = transformCache.TransformDirection(Vector3.forward);
        isHit = Physics.Raycast(transformCache.position, fwd, out hit, 100);
        if (isHit)
        {
            var p = Vector3.Lerp(transformCache.position, hit.point, 0.85f);
            pointer.position = Vector3.Lerp(pointer.position, p, lerpSpeed * Time.deltaTime);
        }
        else
        {
            pointer.localPosition = Vector3.Lerp(pointer.localPosition, oriPointerPosition, lerpSpeed * Time.deltaTime);
        }
    }

    void SetTouchPosition(Vector2 screenPosition)
    {
        // touchPosition.localPosition = new Vector3(screenPosition.x * lineUnit, screenPosition.y * lineUnit, 0.00378f);
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1 - screenPosition.y) * -10 + 0.5f));
    }

}
