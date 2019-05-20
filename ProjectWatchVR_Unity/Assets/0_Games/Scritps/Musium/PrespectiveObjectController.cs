using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class PrespectiveObjectController : MonoBehaviour
{

    public GameObject grabbed_object;

    public float startScale;
    Rigidbody grabbed_object_rigibody;

    void Start()
    {
        Observable.EveryUpdate()
        .Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
        {
            if (grabbed_object == null)
            {
                return;
            }
            if (isGrabing == false)
            {
                grabbed_object_rigibody = grabbed_object.GetComponent<Rigidbody>();
                grabbed_object_rigibody.isKinematic = true;
                // scale = grabbed_object.transform.localScale;
                startScale = grabbed_object.transform.localScale.x / ((Camera.main.transform.position - grabbed_object.transform.position).magnitude);
                isGrabing = true;
                return;
            }


        });

        Observable.EveryUpdate()
        .Where(_ => Input.GetMouseButtonUp(0)).Subscribe(_ =>
        {
            if (grabbed_object == null)
            {
                return;
            }
            if (isGrabing == true)
            {
                grabbed_object_rigibody = grabbed_object.GetComponent<Rigidbody>();

                grabbed_object_rigibody.isKinematic = false;
                isGrabing = false;
                return;
            }
        });
    }
    void Update()
    {
        if (isGrabing == false)
        {
            grabbed_object = CheckForGrableObject();

            if (grabbed_object == null)
            {
                return;
            }
        }
        else
        {
            UpdateGrabObjectPosition();
            GetScaleRatio();
        }
    }

    void GetScaleRatio()
    {
        var vectorAllCameraToHitPoint = Camera.main.transform.position - lastPosition;
        var s = startScale * vectorAllCameraToHitPoint.magnitude;
        grabbed_object.transform.localScale = new Vector3(s, s, s);
    }
    public float lerpSpeed = 5f;

    private void UpdateGrabObjectPosition()
    {
        grabbed_object_rigibody.position = GetTargetPosition();
    }

    public bool isGrabing = false;
    public LayerMask layerMaskForGrableObject;

    private GameObject CheckForGrableObject()
    {
        RaycastHit hitInfo = default(RaycastHit);
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, float.PositiveInfinity, layerMaskForGrableObject))
        {
            GameObject go = hitInfo.collider.gameObject;
            return go;
        }
        return null;
    }
    public LayerMask layerMaskForWallDetect;

    Vector3 lastPosition;
    private Vector3 GetTargetPosition()
    {
        RaycastHit hitInfo = default(RaycastHit);
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, float.PositiveInfinity, layerMaskForWallDetect))
        {
            lastPosition = hitInfo.point;
        }
        return lastPosition;
    }
}
