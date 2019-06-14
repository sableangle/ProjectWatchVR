using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_FrameLevel : MonoBehaviour
{
    public static AI_FrameLevel Instance;
    Transform mainCameraTransform;
    [SerializeField]
    Transform oldRoomCamera;

    Camera _oldRoomCamera;
    public Camera oldRoomC{
        get{
            if(_oldRoomCamera == null){
                _oldRoomCamera = oldRoomCamera.GetComponent<Camera>();
            }
            return _oldRoomCamera;
        }
    }

    Collider trigger;
    void Awake(){
        Instance = this;
    }
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        trigger = GetComponent<Collider>();
        trigger.enabled = false;
    }

    [SerializeField]
    Renderer[] frameRenderer;
    [SerializeField]
    Transform frameRoot;

    float durationTime = 1;
    void StartFrameLevel()
    {
        Sequence inSeq = DOTween.Sequence();
        frameRoot.localPosition = new Vector3(0, -5, 0);
        frameRoot.DOLocalMoveY(0, durationTime);
        foreach (var item in frameRenderer)
        {
            item.material.SetFloat("_DissolveAmount", 1);
            inSeq.Join(item.material.DOFloat(0, "_DissolveAmount", durationTime));
        }

        inSeq.OnComplete(
            () =>
            {
                trigger.enabled = true;
            }
        );
    }

    void EndFrameLevel()
    {
        Sequence inSeq = DOTween.Sequence();
        frameRoot.localPosition = new Vector3(0, 0, 0);
        frameRoot.DOLocalMoveY(-5, durationTime);
        foreach (var item in frameRenderer)
        {
            item.material.SetFloat("_DissolveAmount", 0);
            inSeq.Join(item.material.DOFloat(1, "_DissolveAmount", durationTime));
        }
    }

    bool isPlayerIn = false;
    [SerializeField]
    Transform oldRoomOriLocalPosition;
    [SerializeField]
    Transform frameOriTransform;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartFrameLevel();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            EndFrameLevel();
        }

        if (!isPlayerIn)
        {
            return;
        }

        var offset = frameOriTransform.position - mainCameraTransform.position;
        offset.y *= -1;
        var localOffect = frameOriTransform.InverseTransformPoint(offset);
        localOffect.y = offset.y;
        oldRoomCamera.localPosition = Vector3.Lerp(oldRoomCamera.localPosition, localOffect, 8 * Time.deltaTime);
        oldRoomCamera.rotation = Quaternion.Lerp(oldRoomCamera.rotation, mainCameraTransform.rotation, 8 * Time.deltaTime);


    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
        {
            return;
        }
        isPlayerIn = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
        {
            return;
        }
        isPlayerIn = false;
    }
}
