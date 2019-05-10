using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowPrePass : MonoBehaviour
{
    private static RenderTexture PrePass;
    private static RenderTexture Blurred;
    public Camera sourceCamera;
    private Material _blurMat;

    void OnEnable()
    {
        sourceCamera = transform.parent.GetComponent<Camera>();
        PrePass = new RenderTexture(Screen.width, Screen.height, 24);
        PrePass.antiAliasing = QualitySettings.antiAliasing;
        Blurred = new RenderTexture(Screen.width >> 1, Screen.height >> 1, 0);

        var camera = GetComponent<Camera>();
        var glowShader = Shader.Find("Hidden/GlowReplace");
        camera.targetTexture = PrePass;
        camera.SetReplacementShader(glowShader, "Glowable");
        Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
        camera.fieldOfView = sourceCamera.fieldOfView;
        Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);

        _blurMat = new Material(Shader.Find("Hidden/Blur"));
        _blurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * 1.5f, Blurred.texelSize.y * 1.5f));

    }
    void OnPreRender()
    {
        if (PrePass == null || PrePass.width != sourceCamera.pixelWidth || PrePass.height != sourceCamera.pixelHeight)
        {
            PrePass = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            Blurred = new RenderTexture(sourceCamera.pixelWidth >> 1, sourceCamera.pixelHeight >> 1, 0);

            var camera = GetComponent<Camera>();
            camera.targetTexture = PrePass;
            camera.fieldOfView = sourceCamera.fieldOfView;
            Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
            Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
        }
    }
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst);

        Graphics.SetRenderTarget(Blurred);
        GL.Clear(false, true, Color.clear);

        Graphics.Blit(src, Blurred);

        for (int i = 0; i < 4; i++)
        {
            var temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
            Graphics.Blit(Blurred, temp, _blurMat, 0);
            Graphics.Blit(temp, Blurred, _blurMat, 1);
            RenderTexture.ReleaseTemporary(temp);
        }
    }

}
