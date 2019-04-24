using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearController : MonoBehaviour
{
    public enum HandType
    {
        Right, Left
    }
    public HandType CurrentHand;
    public GameObject[] Hands;
    private Transform transformCache;
    public Material screenMaterial;
    public bool editorSimlator = false;

    void Awake()
    {
        transformCache = transform;
        foreach (var item in Hands)
        {
            item.SetActive(false);
        }

        if (CurrentHand == HandType.Right)
        {
            Hands[0].SetActive(true);
        }
        else
        {
            Hands[1].SetActive(true);
        }
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
        ProcessPick();
    }

    Transform oriPickerParent;
    Vector3 targetPointerPosition;
    bool isPicking
    {
        get
        {
            return pointer.childCount > 0;
        }
    }
    void ProcessPick()
    {
        if (lastPickable == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            oriPickerParent = lastPickable.transform.parent;
            targetPointerPosition = pointer.localPosition;
            lastPickable.OnPickStart();
            lastPickable.transform.SetParent(pointer);
        }
        if (Input.GetMouseButtonUp(0))
        {
            lastPickable.OnPickFinish();
            lastPickable.transform.SetParent(oriPickerParent);
        }
        if (isPicking)
        {
            targetPointerPosition += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel")) * 1;
            pointer.localPosition = Vector3.Lerp(
                pointer.localPosition,
                targetPointerPosition,
                lerpSpeed * Time.deltaTime);
        }
    }

    public Transform pointer;
    private Vector3 pointerTargetScale = new Vector3(4.5f, 4.5f, 0.045f);

    private Vector3 oriPointerPosition = new Vector3(0, 0, -0.6f);
    private bool isHit = false;
    private RaycastHit hit;
    public float lerpSpeed = 10;

    PickableObject lastPickable;
    void RayCast()
    {
        if (isPicking == true)
        {
            return;
        }
        Vector3 fwd = transformCache.TransformDirection(Vector3.forward);
        isHit = Physics.Raycast(transformCache.position, fwd, out hit, 100);
        if (isHit)
        {
            var p = Vector3.Lerp(transformCache.position, hit.point, 0.85f);
            pointer.position = Vector3.Lerp(pointer.position, p, lerpSpeed * Time.deltaTime);
            pointer.localScale = Vector3.Lerp(pointer.localScale, pointerTargetScale, lerpSpeed * Time.deltaTime);

            var g = hit.collider.gameObject;
            if (!g.CompareTag("Pickable"))
            {
                ResetCurrentPickable();
                return;
            }
            var pickable = g.GetComponent<PickableObject>();
            if (pickable == lastPickable)
            {
                return;
            }
            ResetCurrentPickable();
            lastPickable = pickable;
            lastPickable.OnPointEnter();
        }
        else
        {
            ResetCurrentPickable();
            pointer.localScale = Vector3.Lerp(pointer.localScale, Vector3.zero, lerpSpeed * Time.deltaTime);
            pointer.localPosition = Vector3.Lerp(pointer.localPosition, oriPointerPosition, lerpSpeed * Time.deltaTime);
        }
    }
    void ResetCurrentPickable()
    {
        if (lastPickable != null)
        {
            lastPickable.OnPointOut();
            lastPickable = null;
        }
    }
    void SetTouchPosition(Vector2 screenPosition)
    {
        // touchPosition.localPosition = new Vector3(screenPosition.x * lineUnit, screenPosition.y * lineUnit, 0.00378f);
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1 - screenPosition.y) * -10 + 0.5f));
    }

}
