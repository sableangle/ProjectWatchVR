﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class UI_Tutorial : MonoBehaviour
{
    [SerializeField]
    UITextTypeWriter typeWriter;

    [SerializeField]
    GameObject nextHint;

    void Start()
    {
        this.ObserveEveryValueChanged(m => m.isTyping).Subscribe(
            (m) =>
            {
                nextHint.SetActive(!m);
            }
        );
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
            {
                GoNext();
            });

        VRInputReciver.OnWatchButtonDown += OnWatchButtonDown;
        VRInputReciver.OnWatchButtonUp += OnWatchButtonUp;

        StartCoroutine(StartTutorial());
        hintImage.color = new Color(1, 1, 1, 0);
    }

    private void OnWatchButtonUp(VRInputReciver.Buttons btn)
    {

    }

    private void OnWatchButtonDown(VRInputReciver.Buttons btn)
    {
        if (btn == VRInputReciver.Buttons.Center) UnityMainThreadDispatcher.Instance().Enqueue(GoNext);
    }

    void GoNext()
    {
        if (i == 1)
            next = true;
    }

    string[] text = new[]{
        "嗨，歡迎你來\n現在將帶領你進行一些基本操作",
        "按住手錶的下緣可以校正控制器",
        "123123123123"
    };

    bool next = false;
    int i = 0;

    IEnumerator StartTutorial()
    {
        yield return typeWriter.StartShow(text[i]);
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(0);
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
    }

    [SerializeField]
    Sprite[] hintImageSprite;
    [SerializeField]
    Image hintImage;
    YieldInstruction ShowHintImage(int index)
    {
        hintImage.sprite = hintImageSprite[index];
        hintImage.color = new Color(1, 1, 1, 0);
        Sequence seq = DOTween.Sequence();
        seq.Join(hintImage.DOFade(1,0.3f));
        seq.Join(hintImage.rectTransform.DOAnchorPosY(-23.2f,0.3f));
        return seq.WaitForCompletion();
    }

    bool isTyping = false;
    public void OnTypeStart()
    {
        isTyping = true;
        next = false;
    }
    public void OnTypeFinish()
    {
        isTyping = false;
        i++;

    }

}