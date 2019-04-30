using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
public class WearController : MonoBehaviour
{

    [Header("開啟編輯器滑鼠模擬")]
    public bool editorSimlator = false;

    public static WearController Instance;
    public enum HandType
    {
        Right, Left
    }
    public HandType CurrentHand;
    public GameObject[] Hands;
    private Transform transformCache;
    public Material screenMaterial;

    void Awake()
    {
        Instance = this;
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
        if (editorSimlator)
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
                {
                    PickStart();
                });
            Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0)).Subscribe(_ =>
            {
                PickEnd();
            });
        }
        else
        {
            VRInputReciver.OnWatchButtonDown += OnWatchButtonDown;
            VRInputReciver.OnWatchButtonUp += OnWatchButtonUp;
        }
    }
    private void OnWatchButtonUp(VRInputReciver.Buttons btn)
    {
        //Debug.Log("OnWatchButtonUp " + btn);
        if (btn == VRInputReciver.Buttons.Center) UnityMainThreadDispatcher.Instance().Enqueue(PickEnd);
        if (btn == VRInputReciver.Buttons.Right) _flishlight = false;
        if (btn == VRInputReciver.Buttons.Down)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UI_ScreenSpace.Instance.ExitHint_Reset();
            });
            _resetTimerSwitch = false;
        }
        if (btn == VRInputReciver.Buttons.Up)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UI_ScreenSpace.Instance.ExitHint_Setting();
            });
            _settingHintSwitch = false;
        }
    }

    private void OnWatchButtonDown(VRInputReciver.Buttons btn)
    {
        //Debug.Log("OnWatchButtonDown " + btn);
        if (btn == VRInputReciver.Buttons.Center)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(PickStart);
        }
        if (btn == VRInputReciver.Buttons.Right)
        {
            lastScreenPosY = VRInputReciver.screenPosition.y;
            _flishlight = true;
        }
        if (btn == VRInputReciver.Buttons.Down)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UI_ScreenSpace.Instance.ShowHint_Reset();
            });
            _resetTimerSwitch = true;
        }
        if (btn == VRInputReciver.Buttons.Up)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UI_ScreenSpace.Instance.ShowHint_Setting();
            });
            _settingHintSwitch = true;
        }
    }
    //Triggers

    bool _flishlight = false;
    bool GetFlishlightOpen()
    {
        if (editorSimlator)
        {
            return Input.GetKey(KeyCode.J);
        }
        else
        {
            return _flishlight;
        }
    }
    bool _resetTimerSwitch = false;
    float _resetTimer = 0;
    public float GetResetSensorTimer()
    { return _resetTimer; }
    public bool GetResetSensor()
    {
        if (editorSimlator)
        {
            return Input.GetKey(KeyCode.G);
        }
        else
        {
            return _resetTimerSwitch;
        }
    }
    float _settingTimer = 0;

    bool _settingHintSwitch = false;
    public float GetSettingTimer()
    { return _settingTimer; }
    public bool GetSettingSensor()
    {
        if (editorSimlator)
        {
            return Input.GetKey(KeyCode.Y);
        }
        else
        {
            return _settingHintSwitch;
        }
    }


    float lastScreenPosY;
    float getScreenMoven()
    {
        if (editorSimlator)
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        else
        {
            float result = lastScreenPosY - VRInputReciver.screenPosition.y;
            lastScreenPosY = VRInputReciver.screenPosition.y;
            return result * 10;
        }
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
            // transformCache.rotation = VRInputReciver.rotation;
            // transformCache.eulerAngles = new Vector3(transformCache.eulerAngles.x, transformCache.eulerAngles.y - transformCache.parent.eulerAngles.y, VRInputReciver.accelerometer.x * 10);
            transformCache.rotation = Quaternion.Lerp(
                transformCache.rotation,
                Quaternion.Euler(VRInputReciver.rotation.eulerAngles.x, VRInputReciver.rotation.eulerAngles.y - transformCache.parent.eulerAngles.y, VRInputReciver.accelerometer.x * 10),
                lerpSpeedForRotation * Time.deltaTime);

            SetTouchPosition(VRInputReciver.screenPosition);
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
        FlashLight();
        ResetSensor();
        SettingHint();
    }
    void ResetSensor()
    {
        if (GetResetSensor())
        {
            _resetTimer += Time.deltaTime;
            if (_resetTimer > GlobalDefine.resetNeedTime)
            {
                WebScoketServer.Instance.SendMsg("Reset Sensor");
                _resetTimerSwitch = false;
            }
        }
        else
        {
            _resetTimer = 0;
        }
    }
    void SettingHint()
    {
        if (GetSettingSensor())
        {
            _settingTimer += Time.deltaTime;
            if (_settingTimer > GlobalDefine.settingNeedTime)
            {
                //WebScoketServer.Instance.SendMsg("Reset Sensor");
                _settingHintSwitch = false;
            }
        }
        else
        {
            _settingTimer = 0;
        }
    }
    // void OnGUI()
    // {
    //     GUILayout.BeginVertical();
    //     foreach (VRInput.Buttons suit in (VRInput.Buttons[])System.Enum.GetValues(typeof(VRInput.Buttons)))
    //     {
    //         GUILayout.Label(suit.ToString() + " Down : " + VRInput.GetWatchButtonDown(suit));
    //         GUILayout.Label(suit.ToString() + " Up : " + VRInput.GetWatchButtonUp(suit));
    //         GUILayout.Label(suit.ToString() + " Hold : " + VRInput.GetWatchButton(suit));
    //     }
    //     GUILayout.EndVertical();
    // }

    [SerializeField]
    private Transform flashlight;
    private Vector3 flashlightPosition = new Vector3(0, 0, 1.5f);
    private Vector3 flashlightOpensize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 flashlightTargetSize = new Vector3(0, 0, 0);
    void FlashLight()
    {
        if (GetFlishlightOpen())
        {
            flashlightTargetSize = flashlightOpensize;
        }
        else
        {
            flashlightTargetSize = Vector3.zero;
        }
        flashlightPosition = new Vector3(0, 0, Mathf.Clamp(flashlightPosition.z + getScreenMoven() * 0.2f, 0.5f, 2f));
        flashlight.localPosition = Vector3.Lerp(
            flashlight.localPosition,
            flashlightPosition,
            lerpSpeed * Time.deltaTime);
        flashlight.localScale = Vector3.Lerp(flashlight.localScale, flashlightTargetSize, lerpSpeed * Time.deltaTime);
    }

    Transform pickWrapper;
    Vector3 targetPointerPosition;
    bool isPicking = false;

    // bool isPicking
    // {
    //     get
    //     {
    //         return pointer.childCount > 0;
    //     }
    // }
    void PickStart()
    {
        if (lastPickable == null)
        {
            return;
        }
        if (pickWrapper == null)
        {
            pickWrapper = new GameObject("PickWrapper").transform;
        }
        pickWrapper.position = lastPickable.transform.position;
        pickWrapper.SetParent(pointer);
        isPicking = true;
        targetPointerPosition = pointer.localPosition;
        lastPickable.OnPickStart(pickWrapper);
    }
    void PickEnd()
    {
        if (lastPickable == null)
        {
            return;
        }
        isPicking = false;
        lastPickable.OnPickFinish();
    }

    void ProcessPick()
    {
        if (isPicking)
        {
            targetPointerPosition = new Vector3(0, 0, Mathf.Clamp(targetPointerPosition.z + getScreenMoven(), 0, 6f));
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
    public float lerpSpeedForRotation = 20;

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
