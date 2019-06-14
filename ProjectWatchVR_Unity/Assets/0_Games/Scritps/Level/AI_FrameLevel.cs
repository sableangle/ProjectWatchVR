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
    public Camera oldRoomC
    {
        get
        {
            if (_oldRoomCamera == null)
            {
                _oldRoomCamera = oldRoomCamera.GetComponent<Camera>();
            }
            return _oldRoomCamera;
        }
    }

    Collider trigger;
    void Awake()
    {
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
        frameRoot.localPosition = new Vector3(0, -3, 0);
        frameRoot.DOLocalMoveY(2, durationTime);
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
        frameRoot.localPosition = new Vector3(0, 2, 0);
        frameRoot.DOLocalMoveY(-3, durationTime);
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
        PaintRayCast();
        UpdatePick();
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


    Renderer hitRenderer;
    Transform hitTransform;
    Ray mRay;
    float length = 0;

    bool isPick = false;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(mRay);
    }

    float clearDis
    {
        get
        {
            if (hitTransform == null)
            {
                return 0;
            }
            return hitTransform.localScale.x *0.5f - oldRoomC.nearClipPlane + 0.05f;
        }
    }
    GameObject Templete;
    void UpdatePick()
    {
        if (isPick == false)
        {
            return;
        }

        length += Input.GetAxis("Mouse ScrollWheel") * 0.5f;
        length = Mathf.Clamp(length, 0, length);

        if (length < clearDis)
        {
            hitTransform.localScale *= 0.9f;
        }
        else{
            hitTransform.localScale = Vector3.one;
        }


        var tPoint = oldRoomC.transform.position + mRay.direction * length;
        if (hitTransform == null)
        {
            return;
        }

        hitTransform.position = Vector3.Lerp(hitTransform.position, tPoint, 10 * Time.deltaTime);
        var tPoint2 = mainCameraTransform.transform.position + mRay.direction * length;
        Templete.transform.position = Vector3.Lerp(Templete.transform.position, tPoint2, 10 * Time.deltaTime);

    }


    public float perfixScale = 0.85f;
    void PaintRayCast()
    {
        if (AI_FrameLevel.Instance == null)
        {
            Debug.Log("AI_FrameLevel.Instance == null");
            return;
        }

        RaycastHit hitInfo;
        Vector3 fwd = WearController.Instance.transformCache.TransformDirection(Vector3.forward);
        bool isHit = Physics.Raycast(WearController.Instance.transformCache.position, fwd, out hitInfo, 100);

        if (!isHit)
        {
            //Debug.Log("!isHit");
            return;
        }

        if (hitInfo.collider.name != "painting")
        {
            //Debug.Log("hitInfo.collider.name != painting");
            return;
        }
        mRay = oldRoomC.ViewportPointToRay(new Vector3(hitInfo.textureCoord.x, hitInfo.textureCoord.y, 0));

        if (Input.GetMouseButtonDown(0))
        {
            if (hitRenderer == null) { return; }
            length = (oldRoomCamera.position - hitRenderer.transform.position).magnitude;
            isPick = true;
            hitTransform = hitRenderer.transform;
            hitTransform.GetComponent<Rigidbody>().isKinematic = true;
            Templete = Instantiate(hitTransform.gameObject);
            Templete.transform.position = mainCameraTransform.position + mRay.direction * length;
            Templete.transform.localScale *= perfixScale;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPick = false;
            hitTransform.GetComponent<Rigidbody>().isKinematic = false;
            hitTransform = null;
            if (Templete)
            {
                Destroy(Templete);
                Templete = null;
            }
        }


        var localPoint = hitInfo.textureCoord;
        RaycastHit hitInfoSec;
        if (Physics.Raycast(mRay, out hitInfoSec))
        {
            // hit should now contain information about what was hit through secondCamera
            if (!hitInfoSec.collider.CompareTag("FrameTarget"))
            {
                //Debug.Log("hitInfo.collider.name != FrameTarget");
                return;
            }

            hitRenderer = hitInfoSec.collider.GetComponent<Renderer>();

            hitRenderer.material.SetFloat("_OutlineWidth", 0.045f);
        }
        else
        {
            if (hitRenderer == null)
            {
                return;
            }

            hitRenderer.material.SetFloat("_OutlineWidth", 0);
            hitRenderer = null;
        }


    }

}
