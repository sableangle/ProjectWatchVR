// =================================
//
//	MobileVRTracker.cs
//	Created by Takuya Himeji
//
// =================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.iOS;

public class MobileVRTracker : MonoBehaviour
{
    #region Singleton
    private static MobileVRTracker instance;

    public static MobileVRTracker Instance
    {
        get { return instance; }
    }
    #endregion // Singleton


    #region Inspector Settings
    [SerializeField] private Camera trackingCamera;  // 対象のカメラ
    #endregion // Inspector Settings


    #region Member Field
    private UnityARSessionNativeInterface session = null;
    private ARKitWorldTrackingSessionConfiguration config;
    private float eyeHeight = 1.7f;   // 床からの高さ(目線)
    #endregion // Member Field


    #region Properties
    /// <summary>
    /// トラッキング対象のCameraのTransform
    /// </summary>
    public Transform TrackingCamera
    {
        get { return trackingCamera.transform; }
    }

    /// <summary>
    /// 底面から目線までの高さ(キャリブレーション値)
    /// </summary>
    public float EyeHeight
    {
        get { return eyeHeight; }
        set { eyeHeight = value; }
    }
    #endregion // Properties

    void InitCamera()
    {
        if (trackingCamera == null)
        {
            trackingCamera = Camera.main;
        }
        trackingCamera.transform.parent.localPosition = new Vector3(0f, eyeHeight, 0f);  // 目線の高さ設定
    }
#if UNITY_EDITOR
    private void Awake()
    {
        instance = this;
        InitCamera();
    }

    private void Update()
    {
        UpdateEditorEmulation();
    }
    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;
    private const string AXIS_MOUSE_X = "Mouse X";
    private const string AXIS_MOUSE_Y = "Mouse Y";
    private const string KEYBOARD_X = "Horizontal";
    private const string KEYBOARD_Z = "Vertical";
    public Quaternion HeadRotation { get; private set; }
    public void UpdateEditorEmulation()
    {
        bool rolled = false;
        if (CanChangeYawPitch())
        {
            mouseX += Input.GetAxis(AXIS_MOUSE_X) * 5;
            if (mouseX <= -180)
            {
                mouseX += 360;
            }
            else if (mouseX > 180)
            {
                mouseX -= 360;
            }

            mouseY -= Input.GetAxis(AXIS_MOUSE_Y) * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -85, 85);
        }
        else if (CanChangeRoll())
        {
            rolled = true;
            mouseZ += Input.GetAxis(AXIS_MOUSE_X) * 5;
            mouseZ = Mathf.Clamp(mouseZ, -85, 85);
        }
        else
        {
        }

        if (CanMove())
        {
            trackingCamera.transform.Translate(new Vector3(Input.GetAxis(KEYBOARD_X), 0, Input.GetAxis(KEYBOARD_Z)) * Time.deltaTime);
        }

        if (!rolled)
        {
            // People don't usually leave their heads tilted to one side for long.
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
        UpdateHeadRotation();
        ApplyHeadOrientationToVRCameras();
    }
    private void UpdateHeadRotation()
    {
        HeadRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
    }
    private void ApplyHeadOrientationToVRCameras()
    {
        // Update all VR cameras using Head position and rotation information.
        trackingCamera.transform.localRotation = HeadRotation;
    }
    private bool CanChangeYawPitch()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }
    private bool CanChangeRoll()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
    private bool CanMove()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

#endif

    // 只有在 iOS 的情況下套用 AR 位移
#if UNITY_IOS && !UNITY_EDITOR
    #region MonoBehaviour Methods
    private void Awake()
    {
        instance = this;                    // インスタンス取得
        Application.targetFrameRate = 60;   // fpsセット
        DontDestroyOnLoad(gameObject);      // 永続化

        // ARKitセッションの初期化 ----
        session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
        config.planeDetection = UnityARPlaneDetection.Horizontal;
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.getPointCloudData = true;
        config.enableLightEstimation = false;
        session.RunWithConfig(config);  // セッション実行

        InitCamera();
        // Cardboardの自動ヘッドトラッキング停止
        UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(trackingCamera, true);

        // シーン遷移時のイベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (trackingCamera != null)
        {
            // 姿勢更新
            Matrix4x4 matrix = session.GetCameraPose();
            trackingCamera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            trackingCamera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);
        }
    }
    #endregion // MonoBehaviour Methodss


    #region Member Methods
    // シーン遷移時にコール
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (trackingCamera == null)
        {
            // カメラの更新(シーン遷移などでカメラが剥がれた時の処理)
            trackingCamera = Camera.main;    // Camera取得
            trackingCamera.transform.parent.localPosition = new Vector3(0f, eyeHeight, 0f);  // 目線の高さ設定
            // Cardboardの自動ヘッドトラッキング停止
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(trackingCamera, true);
        }
    }
    #endregion // Member Methods


#endif
}