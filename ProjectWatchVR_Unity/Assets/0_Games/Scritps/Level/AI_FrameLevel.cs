using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class AI_FrameLevel : MonoBehaviour, ITrigger
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

    [SerializeField]
    MeshFilter mesh;

    List<Vector3> meshVertexWorldPosition = new List<Vector3>();



    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        trigger = GetComponent<Collider>();
        trigger.enabled = false;

        foreach (var pos in mesh.mesh.vertices)
        {
            meshVertexWorldPosition.Add(mesh.transform.TransformPoint(pos));
        }
        StartFrameLevel();
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
            return hitTransform.localScale.x * 0.5f - oldRoomC.nearClipPlane + 0.05f;
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
        else
        {
            hitTransform.localScale = Vector3.one;
        }


        var tPoint = oldRoomC.transform.position + mRay.direction * length;
        if (hitTransform == null)
        {
            return;
        }

        hitTransform.position = Vector3.Lerp(hitTransform.position, tPoint, 10 * Time.deltaTime);
        var tPoint2 = mainCameraTransform.transform.position + mRay.direction * length;

    }


    public float perfixScale = 0.85f;
    Shader outlineShader
    {
        get
        {
            return Shader.Find("Custom/SelfLightWithShadowOutline");
        }
    }
    Shader normalShader
    {
        get
        {
            return Shader.Find("Custom/SelfLightWithShadow");
        }
    }
    Shader stencilShader
    {
        get
        {
            return Shader.Find("Custom/SelfLightWithShadowStencil");
        }
    }
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
            hitRenderer.material.shader = normalShader;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPick = false;
            hitTransform.GetComponent<Rigidbody>().isKinematic = false;
            hitTransform.GetComponent<Renderer>().material.shader = outlineShader;
            hitTransform = null;
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



    public void OnWrapperTriggerEnter(GameObject whoGotHit, Collider other)
    {
        if (!other.CompareTag("FrameTarget"))
        {
            return;
        }
        var objectViewPortPosition = oldRoomC.WorldToViewportPoint(hitTransform.position);

        Templete = Instantiate(hitTransform.gameObject);
        Templete.transform.localScale = Vector3.one * perfixScale;
        Templete.layer = LayerMask.NameToLayer("Ignore Raycast");
        //get the min z position
        mf = Templete.GetComponent<MeshFilter>();
        List<Vector3> instantiateMeshVertexWorldPosition = new List<Vector3>();
        foreach (var p in mf.mesh.vertices) { instantiateMeshVertexWorldPosition.Add(Templete.transform.TransformPoint(p)); }
        objectZoffect = Templete.transform.position.z - instantiateMeshVertexWorldPosition.Min(m => m.z);

        Templete.transform.rotation = hitTransform.rotation;
        //Templete.GetComponent<Renderer>().material.shader = stencilShader;
    }
    MeshFilter mf;
    float objectZoffect;
    public void OnWrapperTriggerStay(GameObject whoGotHit, Collider other)
    {
        if (Templete == null || hitTransform == null)
        {
            return;
        }
        var dir = (WatchLaserPointer.Instance.hitPoint.transform.position - mainCameraTransform.position).normalized;
        //var startPosition = WearController.Instance.pointer.position - (oldRoomCamera.position - hitTransform.position).normalized * objectZoffect * 1 / perfixScale;
        var startPosition = new Vector3(WatchLaserPointer.Instance.hitPoint.transform.position.x, WatchLaserPointer.Instance.hitPoint.transform.position.y, meshVertexWorldPosition.Max(m => m.z) + objectZoffect);

        var a = (hitTransform.position - oldRoomCamera.position).magnitude;
        var offect = a - objectZoffect * 1 / perfixScale - oldRoomC.nearClipPlane;
        // Debug.Log("dir " + dir);
        // Debug.Log("a " + a);
        // Debug.Log("offect " + offect);

        Templete.transform.position = startPosition + dir * offect;
    }

    public void OnWrapperTriggerExit(GameObject whoGotHit, Collider other)
    {
        if (Templete)
        {
            Destroy(Templete);
            Templete = null;
        }
    }
}
