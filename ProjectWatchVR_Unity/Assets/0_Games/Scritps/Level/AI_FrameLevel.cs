using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_FrameLevel : MonoBehaviour
{
    Transform mainCameraTransform;
    [SerializeField]
    Transform oldRoomCamera;
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    bool isPlayerIn = false;
    [SerializeField]
    Transform oldRoomOriLocalPosition;
    [SerializeField]
    Transform frameOriTransform;
    void Update()
    {
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
