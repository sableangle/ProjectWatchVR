using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UniRx;
using System;

public class AI_FrameLevel : MonoBehaviour, ITrigger
{
    public static AI_FrameLevel Instance;
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
        gameObject.SetActive(false);
        RememberResetData();
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

        VRInputReciver.OnWatchButtonDown += OnWatchButtonDown;
        VRInputReciver.OnWatchButtonUp += OnWatchButtonUp;
    }

    private void OnWatchButtonUp(VRInputReciver.Buttons btn)
    {
        if (btn == VRInputReciver.Buttons.Center)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(PickEnd);
        }
    }
    private void OnWatchButtonDown(VRInputReciver.Buttons btn)
    {
        if (btn == VRInputReciver.Buttons.Center)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(PickStart);
        }
    }

    void PickStart()
    {
        if (hitTransform == null) { return; }
        length = (oldRoomCamera.position - hitTransform.position).magnitude;
        isPick = true;
        hitTransform.GetComponent<Rigidbody>().isKinematic = true;
        hitTransform.GetComponent<Renderer>().material.shader = normalShader;
        //hitRenderer.material.shader = normalShader;
    }
    void PickEnd()
    {
        isPick = false;
        if (hitTransform == null) { return; }
        hitTransform.GetComponent<Rigidbody>().isKinematic = false;
        hitTransform.GetComponent<Renderer>().material.shader = outlineShader;
        hitTransform = null;
    }

    [SerializeField]
    Renderer[] frameRenderer;
    [SerializeField]
    Transform frameRoot;

    float durationTime = 1;

    [SerializeField]
    UI_WorldItem hintUI;
    public void StartFrameLevel()
    {
        ResetPosition();
        Sequence inSeq = DOTween.Sequence();
        frameRoot.localPosition = new Vector3(0, -3, 0);
        frameRoot.DOLocalMoveY(2, durationTime);
        foreach (var item in frameRenderer)
        {
            item.material.SetFloat("_DissolveAmount", 1);
            inSeq.Join(item.material.DOFloat(0, "_DissolveAmount", durationTime));
        }

        inSeq.OnStart(
            () =>
            {
                gameObject.SetActive(true);
            }
        );
        inSeq.OnComplete(
            () =>
            {
                trigger.enabled = true;
            }
        );
        hintUI.Show();
        hintUI.transform.localPosition = new Vector3(12.94f, 7.09f, 0);
    }

    public void EndFrameLevel()
    {
        Sequence inSeq = DOTween.Sequence();
        frameRoot.localPosition = new Vector3(0, 2, 0);
        frameRoot.DOLocalMoveY(-3, durationTime);
        foreach (var item in frameRenderer)
        {
            item.material.SetFloat("_DissolveAmount", 0);
            inSeq.Join(item.material.DOFloat(1, "_DissolveAmount", durationTime));
        }

        inSeq.OnComplete(
            () =>
            {
                gameObject.SetActive(false);
            }
        );
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

    [SerializeField]
    Transform hitTransform;
    Ray mRay;
    float length = 0;

    bool isPick = false;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(mRay);
    }

    GameObject Templete;
    void UpdatePick()
    {
        if (isPick == false)
        {
            return;
        }

        length += WearController.Instance.getScreenMoven() * (WearController.Instance.editorSimlator ? 0.5f : 3);
        length = Mathf.Clamp(length, 0, length);


        var tPoint = oldRoomC.transform.position + mRay.direction * length;
        if (hitTransform == null)
        {
            return;
        }

        hitTransform.position = Vector3.Lerp(hitTransform.position, tPoint, 10 * Time.deltaTime);
        var tPoint2 = mainCameraTransform.transform.position + mRay.direction * length;


        if (Templete == null && !startTeleport)
        {
            return;
        }
        var dir = (WatchLaserPointer.Instance.pointer.position - mainCameraTransform.position).normalized;
        //var startPosition = WearController.Instance.pointer.position - (oldRoomCamera.position - hitTransform.position).normalized * objectZoffect * 1 / perfixScale;
        var startPosition = new Vector3(WatchLaserPointer.Instance.pointer.position.x, WatchLaserPointer.Instance.pointer.position.y, meshVertexWorldPosition.Max(m => m.z) + objectZoffect);
        var a = (hitTransform.position - oldRoomCamera.position).magnitude;
        var offect = a - objectZoffect * 1 / perfixScale - oldRoomC.nearClipPlane;

        Templete.transform.position = startPosition + dir * offect;
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
            if (isPick)
            {
                PickEnd();
            }
            return;
        }

        if (hitInfo.collider.name != "painting")
        {
            //Debug.Log("hitInfo.collider.name != painting");
            if (isPick)
            {
                PickEnd();
            }
            return;
        }
        mRay = oldRoomC.ViewportPointToRay(new Vector3(hitInfo.textureCoord.x, hitInfo.textureCoord.y, 0));

        var localPoint = hitInfo.textureCoord;
        RaycastHit hitInfoSec;
        if (isPick)
        {
            return;
        }
        if (Physics.Raycast(mRay, out hitInfoSec))
        {
            // hit should now contain information about what was hit through secondCamera
            if (!hitInfoSec.collider.CompareTag("FrameTarget"))
            {
                TryResetOutline();
                hitTransform = null;
                return;
            }
            hitTransform = hitInfoSec.collider.transform;
            hitTransform.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.045f);
        }
        else
        {
            TryResetOutline();
            hitTransform = null;
        }

    }

    void TryResetOutline()
    {
        try
        {
            hitTransform.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0);
        }
        catch
        {

        }
    }

    bool startTeleport = false;
    public void OnWrapperTriggerEnter(GameObject whoGotHit, Collider other)
    {
        if (whoGotHit.name == "Trigger")
        {
            if (!other.CompareTag("FrameTarget"))
            {
                return;
            }

            if (!isPick)
            {
                return;
            }
            Templete.SetActive(false);
            var go = Instantiate(Templete, Templete.transform.position, Templete.transform.rotation);
            go.SetActive(true);
            go.transform.localScale = Templete.transform.localScale;
            go.tag = "Pickable";
            go.GetComponent<Rigidbody>().isKinematic = false;
            var mutil = go.AddComponent<MutilFunctionObject>();
            hitTransform.gameObject.SetActive(false);

            PickEnd();

        }

        if (whoGotHit.name == "OldRoomCamera")
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
            startTeleport = true;
        }


    }

    [SerializeField]
    Transform[] interactiveObejct;
    void RememberResetData()
    {
        for (int i = 0; i < interactiveObejct.Length; i++)
        {
            var temp = new ResetData();
            temp.pos = interactiveObejct[i].position;
            temp.scale = interactiveObejct[i].localScale;
            temp.rot = interactiveObejct[i].eulerAngles;
            resetData.Add(temp);
        }
    }
    void ResetPosition()
    {
        for (int i = 0; i < interactiveObejct.Length; i++)
        {
            interactiveObejct[i].gameObject.SetActive(true);
            interactiveObejct[i].position = resetData[i].pos;
            interactiveObejct[i].localScale = resetData[i].scale;
            interactiveObejct[i].eulerAngles = resetData[i].rot;
        }
    }
    List<ResetData> resetData = new List<ResetData>();
    class ResetData
    {
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 scale;
    }

    MeshFilter mf;
    float objectZoffect;
    public void OnWrapperTriggerStay(GameObject whoGotHit, Collider other)
    {

    }

    public void OnWrapperTriggerExit(GameObject whoGotHit, Collider other)
    {
        startTeleport = false;

        if (Templete)
        {
            Destroy(Templete);
            Templete = null;
        }
    }
}
