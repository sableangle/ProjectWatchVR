﻿using System.Collections;
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
    public Transform transformCache;
    public Material screenMaterial;
    Quaternion initRot;
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
        initRot = transformCache.localRotation;

    }
    void Start()
    {
#if !UNITY_EDITOR
        editorSimlator = false;
        transform.localPosition = new Vector3(0, 0, 0);
#else
        transform.localPosition = new Vector3(0, 0, 0.5f);
#endif
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
            Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.U)).Subscribe(_ =>
            {
                ChangeMutilFunction();
            });

            Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.G)).Subscribe(_ =>
            {
                _resetTimerSwitch = Input.GetKey(KeyCode.G);
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
        //if (btn == VRInputReciver.Buttons.Right) _flishlight = false;
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

        if (btn == VRInputReciver.Buttons.Left)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UI_MainMenu.Instance.Swtich();

            });
        }
    }

    public delegate void _OnObjectChangeFunction();
    public _OnObjectChangeFunction OnObjectChangeFunction;

    private void OnWatchButtonDown(VRInputReciver.Buttons btn)
    {
        lastScreenPosY = VRInputReciver.screenPosition.y;

        //Debug.Log("OnWatchButtonDown " + btn);
        if (btn == VRInputReciver.Buttons.Center)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(PickStart);
        }
        if (btn == VRInputReciver.Buttons.Right)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                ChangeMutilFunction();
            });
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

    void ChangeMutilFunction()
    {
        if (lastPickable == null)
        {
            return;
        }
        if (!(lastPickable is MutilFunctionObject))
        {
            return;
        }
                ((MutilFunctionObject)lastPickable).ChangeFunction();
        if (OnObjectChangeFunction != null)
        {
            OnObjectChangeFunction();
        }
    }
    //Triggers

    bool _flishlight = false;
    bool GetFlishlightOpen()
    {
        if (editorSimlator)
        {
            //return true;
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
    public float getScreenMoven()
    {
        if (editorSimlator)
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        else
        {
            float result = lastScreenPosY - VRInputReciver.screenPosition.y;
            lastScreenPosY = VRInputReciver.screenPosition.y;
            return result * 5;
        }
    }

    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;
    private const string AXIS_MOUSE_X = "Mouse X";
    private const string AXIS_MOUSE_Y = "Mouse Y";
    public Quaternion WearRotation { get; private set; }
    [SerializeField, Range(0.01f, 1f)]
    public float lerpParam = 0.5f;

    float lastY = 0;
    void Update()
    {

        if (!editorSimlator)
        {
            
            lastY = Mathf.Lerp(lastY, -VRInputReciver.accelerometer.y * 9.8f, lerpSpeedForRotation * Time.deltaTime);
            var targetRotation = initRot * Quaternion.Euler(lastY,
                                    VRInputReciver.rotation.eulerAngles.y,
                                    0);
            transformCache.rotation = Quaternion.Slerp(
                transformCache.rotation,
                targetRotation,
                1 - Mathf.Pow(lerpParam, Time.deltaTime * 60));


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

                WearRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);

                // Update all VR cameras using Head position and rotation information.
                transformCache.localRotation = WearRotation;
            }
        }

        RayCast();
        ProcessPick();
        //FlashLight();
        ResetSensor();
        SettingHint();
        DrawPointerLine();
    }


    public delegate void _OnResetFinish();
    public event _OnResetFinish OnResetFinish;
    void ResetSensor()
    {
        if (GetResetSensor())
        {
            _resetTimer += Time.deltaTime;
            if (_resetTimer > GlobalDefine.resetNeedTime)
            {
                initRot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                WebScoketServer.Instance.SendMsg("Reset Sensor");
                if (OnResetFinish != null) OnResetFinish();
                _resetTimerSwitch = false;
                _resetTimer = 0;
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
    //     GUILayout.EndVertical();
    // }

    [SerializeField]
    private Transform flashlight;
    private Vector3 flashlightPosition = new Vector3(0, 0, 1.5f);
    private Vector3 flashlightOpensize = new Vector3(1.5f, 1.5f, 1.5f);
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
        flashlightPosition = new Vector3(0, 0, 1.5f);

        flashlight.localScale = Vector3.Lerp(flashlight.localScale, flashlightTargetSize, lerpSpeed * Time.deltaTime);
    }

    Transform pickWrapper;
    Vector3 targetPointerPosition;
    public static bool isPicking = false;

    public void PickStart()
    {
        if (lastPickable == null)
        {
            return;
        }
        if (pickWrapper == null)
        {
            pickWrapper = new GameObject("PickWrapper").transform;
        }
        pickWrapper.position = lastPickable.GetTransform().position;
        pickWrapper.SetParent(pointer);
        isPicking = true;
        targetPointerPosition = pointer.localPosition;
        lastPickable.OnPickStart(pickWrapper);
    }
    public void PickEnd()
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

    }
    public void SetCurrentPickable(IPickable pickable)
    {
        lastPickable = pickable;
    }
    [SerializeField]
    WatchLaserPointer watchLaserPointer;

    LineRenderer _lineRenderer;

    LineRenderer lineRenderer
    {
        get
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
            return _lineRenderer;
        }
    }
    void DrawPointerLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, watchLaserPointer.pointer.position);
    }
    public Transform pointer;

    private Vector3 pointerTargetScale = new Vector3(4.5f, 4.5f, 0.045f);

    private Vector3 oriPointerPosition = new Vector3(0, 0, -0.6f);
    private bool isHit = false;
    private RaycastHit hit;
    public float lerpSpeed = 10;
    public float lerpSpeedForRotation = 20;

    IPickable lastPickable;
    void RayCast()
    {
        Vector3 fwd = transformCache.TransformDirection(Vector3.forward);
        isHit = Physics.Raycast(transformCache.position, fwd, out hit, 100);

        if (isPicking)
        {

            if (lastPickable is GroundObject)
            {
                if (isHit && hit.collider.CompareTag("Floor"))
                {
                    targetPointerPosition = new Vector3(0, 0, hit.distance / 1.8f);
                    ((GroundObject)lastPickable).SetIsGround(true);
                }
                else
                {
                    ((GroundObject)lastPickable).SetIsGround(false);
                }
            }
            else
            {
                targetPointerPosition = new Vector3(0, 0, Mathf.Clamp(targetPointerPosition.z + getScreenMoven(), 0, 6f));
            }
            pointer.localPosition = Vector3.Lerp(
                              pointer.localPosition,
                              targetPointerPosition,
                              lerpSpeed * Time.deltaTime);
            return;
        }
        if (isHit)
        {
            var p = Vector3.Lerp(transformCache.position, hit.point, 0.85f);
            pointer.position = Vector3.Lerp(pointer.position, p, lerpSpeed * Time.deltaTime);
            var g = hit.collider.gameObject;
            if (!g.CompareTag("Pickable"))
            {
                ResetCurrentPickable();
                return;
            }
            var pickable = g.GetComponent<IPickable>();
            if (pickable == lastPickable)
            {
                return;
            }
            ResetCurrentPickable();
            lastPickable = pickable;
            lastPickable.OnPointEnter();
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
        screenMaterial.SetTextureOffset("_TouchDotTex", new Vector2(screenPosition.x * -10 + 0.5f, (1 - screenPosition.y) * -10 + 0.5f));
    }

}
