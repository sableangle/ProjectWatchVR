﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AI_PrespectiveLevel : MonoBehaviour
{
    public static AI_PrespectiveLevel Instance;
    [SerializeField]
    CanvasGroup hint;

    [SerializeField]
    GameObject Table;

    [SerializeField]
    GameObject[] mark;

    [SerializeField]
    GameObject[] cube;
    public float durationTime = 1;

    [SerializeField]
    BoxCheck boxCheckBig;
    [SerializeField]
    BoxCheck boxCheckSmall;
    Shader dissolveShader
    {
        get
        {
            return Shader.Find("Custom/SelfLightWithShadowDissolve");
        }
    }
    Shader normalShader
    {
        get
        {
            return Shader.Find("Custom/SelfLightWithShadow");
        }
    }
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        RememberResetData();
    }
    bool finishHint = false;
    void Update()
    {


        if (boxCheckSmall.isPass && boxCheckBig.isPass && finishHint == false)
        {
            finishHint = true;
            hint.GetComponentInChildren<UnityEngine.UI.Text>().text = "酷喔！";
            Invoke("ExitPrespectiveLevel", 2.5f);
            foreach (var item in cube)
            {
                item.GetComponent<MutilFunctionObject>().StopAllInteractive();
            }
        }
    }


    void Start()
    {
        foreach (var item in cube)
        {
            item.SetActive(false);
        }
    }

    [SerializeField]
    UI_WorldItem hintUI;
    public void StartPrespectiveLevel()
    {
        ResetPosition();
        var table_Renderer = Table.GetComponent<Renderer>();

        Sequence tableIn = DOTween.Sequence();
        hint.alpha = 0;

        foreach (var item in table_Renderer.materials)
        {
            item.shader = dissolveShader;
            item.SetFloat("_DissolveAmount", 1);
            tableIn.Join(item.DOFloat(0, "_DissolveAmount", durationTime));
        }
        foreach (var item in mark)
        {
            var r = item.GetComponent<Renderer>();
            r.material.SetFloat("_Alpha", 0);
        }
        tableIn.OnStart(
            () =>
            {
                gameObject.SetActive(true);
            }
        );
        tableIn.OnComplete(
            () =>
            {
                foreach (var item in mark)
                {
                    var r = item.GetComponent<Renderer>();
                    r.material.DOFloat(1, "_Alpha", durationTime);
                }
                hint.DOFade(1, durationTime);
                foreach (var item in cube)
                {
                    item.SetActive(true);
                }
                foreach (var item in table_Renderer.materials)
                {
                    item.shader = normalShader;
                }
            }
        );
        hintUI.Show();
        hintUI.transform.localPosition = new Vector3(0, 2.031f, 2.807f);

    }

    public void ExitPrespectiveLevel()
    {
        var table_Renderer = Table.GetComponent<Renderer>();
        Sequence tableIn = DOTween.Sequence();
        hint.alpha = 0;

        foreach (var item in table_Renderer.materials)
        {
            item.shader = dissolveShader;
            item.SetFloat("_DissolveAmount", 0);
            tableIn.Join(item.DOFloat(1, "_DissolveAmount", durationTime));
        }
        foreach (var item in mark)
        {
            var r = item.GetComponent<Renderer>();
            r.material.SetFloat("_Alpha", 1);
        }
        foreach (var item in mark)
        {
            var r = item.GetComponent<Renderer>();
            r.material.DOFloat(0, "_Alpha", durationTime);
        }
        hint.DOFade(0, durationTime);
        tableIn.OnComplete(
            () =>
            {
                foreach (var item in cube)
                {
                    item.SetActive(false);
                }
                // foreach (var item in table_Renderer.materials)
                // {
                //     item.shader = normalShader;
                // }
                gameObject.SetActive(false);
            }
        );
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


}
