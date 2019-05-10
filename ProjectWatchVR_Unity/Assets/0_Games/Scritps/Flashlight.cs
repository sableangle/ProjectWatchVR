using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Camera mCamera;
    Transform transformCache;
    Vector2 screen = new Vector2(0, 0);
    Material material;
    Mesh m;


    public Camera realWorldCamera;
    public Renderer renderPlane;

    void Start()
    {
        transformCache = transform;
        screen.x = Screen.width;
        screen.y = Screen.height;
        var renderer = GetComponentInChildren<Renderer>();
        var meshFilter = GetComponentInChildren<MeshFilter>();
        // 建立頂點
        Vector3[] v = new Vector3[4];
        v[0] = new Vector3(-0.5f, -0.5f, 0);
        v[1] = new Vector3(0.5f, -0.5f, 0);
        v[2] = new Vector3(-0.5f, 0.5f, 0);
        v[3] = new Vector3(0.5f, 0.5f, 0);

        // 建立三角形
        int[] t = new int[6];
        t[0] = 2;
        t[1] = 1;
        t[2] = 0;

        t[3] = 1;
        t[4] = 2;
        t[5] = 3;

        // 建立法線
        Vector3[] n = new Vector3[4];
        n[0] = new Vector3(0, 0, -1);
        n[1] = new Vector3(0, 0, -1);
        n[2] = new Vector3(0, 0, -1);
        n[3] = new Vector3(0, 0, -1);
        // 套用
        m = new Mesh();

        m.vertices = v;
        m.triangles = t;
        m.normals = n;
        meshFilter.mesh = m;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        m.uv2 = uv;

        var rd = new RenderTexture(Screen.width / 2, Screen.height, 16, RenderTextureFormat.ARGB32);
        realWorldCamera.targetTexture = rd;
        renderPlane.sharedMaterial.SetTexture("_MainTex", rd);
    }
    // Update is called once per frame
    public float uvSize = 0.1f;
    void Update()
    {
        if (mCamera == null)
        {
            mCamera = Camera.main;
        }

        var p = mCamera.WorldToScreenPoint(transformCache.position);
        var screenUV = new Vector2(p.x / screen.x, (p.y) / screen.y);
       // Debug.LogFormat("u: {0} , v: {1}", screenUV.x, screenUV.y);
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(Mathf.Clamp01(screenUV.x - uvSize), Mathf.Clamp01(screenUV.y - uvSize));
        uv[1] = new Vector2(Mathf.Clamp01(screenUV.x + uvSize), Mathf.Clamp01(screenUV.y - uvSize));
        uv[2] = new Vector2(Mathf.Clamp01(screenUV.x - uvSize), Mathf.Clamp01(screenUV.y + uvSize));
        uv[3] = new Vector2(Mathf.Clamp01(screenUV.x + uvSize), Mathf.Clamp01(screenUV.y + uvSize));
        m.uv = uv;
        //material.SetTextureOffset("_MainTex", screenUV);
    }
}
