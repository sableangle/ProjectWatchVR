using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AI_PrespectiveLevel : MonoBehaviour
{
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

    bool finishHint = false;
    void Update()
    {
        

        if (boxCheckSmall.isPass && boxCheckBig.isPass && finishHint == false)
        {
            finishHint = true;
            hint.GetComponentInChildren<UnityEngine.UI.Text>().text = "酷喔！";
            Invoke("ExitPrespectiveLevel", 2.5f);
            foreach(var item in cube){
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
    void StartPrespectiveLevel()
    {
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
    }

    void ExitPrespectiveLevel()
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
            }
        );
    }


}
