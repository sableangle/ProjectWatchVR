﻿using System.Collections;
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

    public float durationTime = 1;

    // Start is called before the first frame update
    void StartPrespectiveLevel()
    {
        var table_Renderer = Table.GetComponent<Renderer>();
        Sequence tableIn = DOTween.Sequence();
        hint.alpha = 0;
       
        foreach (var item in table_Renderer.materials)
        {
            item.SetFloat("_DissolveAmount", 1);
            tableIn.Join(item.DOFloat(0, "_DissolveAmount", durationTime));
        }
        foreach (var item in mark)
        {
            var r = item.GetComponent<Renderer>();
            r.material.SetColor("_TintColor", new Color(0, 1, 0, 0));
        }

        tableIn.OnComplete(
            () =>
            {
                foreach (var item in mark)
                {
                    var r = item.GetComponent<Renderer>();
                    r.material.DOColor(new Color(0, 1, 0, 1), "_TintColor", durationTime);
                }
                hint.DOFade(1,durationTime);
            }
        );
    }

    // Update is called once per frame
    void OnEnable()
    {
        StartPrespectiveLevel();
    }
}
