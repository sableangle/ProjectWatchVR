using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class UI_Tutorial : MonoBehaviour, ITrigger
{
    [SerializeField]
    UITextTypeWriter typeWriter;

    [SerializeField]
    GameObject nextHint;

    [SerializeField]
    Collider tutorialTrigger;
    [SerializeField]
    Collider tutorialMoveTrigger;

    [SerializeField]
    Transform TutorialUI;

    void Start()
    {
        TutorialUI.localPosition = new Vector3(0, 2.76f, 3f);
        tutorialTrigger.enabled = false;
        tutorialMoveTrigger.enabled = false;
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
        WearController.Instance.OnResetFinish += ResetSensor;
        WearController.Instance.OnObjectChangeFunction += OnObjectChangeFunction;
        UI_MainMenu.Instance.OnMenuOpen += MenuTutorial;

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

    void MenuTutorial()
    {
        if (nextHint.activeSelf == false)
        {
            return;
        }
        if (i == 9)
            next = true;
    }
    void GoNext()
    {
        if (nextHint.activeSelf == false)
        {
            return;
        }
        if (i == 1 || i == 3 || i == 4 || i == 8 || i == 10)
            next = true;

    }
    void ResetSensor()
    {
        if (nextHint.activeSelf == false)
        {
            return;
        }
        if (i == 2)
        {
            next = true;
            UI_Toast.Instance.ShowToast("很棒！");
            Debug.Log("ResetSensor");
        }

    }

    private void OnObjectChangeFunction()
    {
        if (nextHint.activeSelf == false)
        {
            return;
        }
        if (i == 7)
        {
            UI_Toast.Instance.ShowToast("很棒！");
            next = true;
        }
    }

    string[] text = new[]{
        "嗨，歡迎你來\n現在將帶領你進行一些基本操作",
        "按住手錶的下緣可以校正控制器",
        "接下來是基本操作，試著揮動控制器",
        "大部分的情況下，控制器的中央代表確認",
        "指向物品時，若物體出現「黃色」外框線時可以中央按鈕將其拿起",
        "物體拿起時可以前後滑動控制器表面前後移動物體",
        "指向物體時，可以按右邊按鈕改變物體模式",
        "「綠色」外框線的物體，在提起時有不同的效果",
        "按下右側按鈕可以開啟選單",
        "教學到此結束，接下來請自由探索吧",
    };
    string[] hint = new[]{
        "按中央鍵繼續",
        "試著完成校正",
        "試著拿起物品",
        "把物品移向自己",
        "改變物品模式",
        "試著開啟選單",
    };

    bool next = false;
    public int i = 0;

    IEnumerator StartTutorial()
    {
        yield return typeWriter.StartShow(text[i]);
        nextHint.GetComponent<Text>().text = hint[0];
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        nextHint.GetComponent<Text>().text = hint[1];
        yield return ShowHintImage(0);
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        nextHint.GetComponent<Text>().text = hint[0];
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(1);
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        GenerateInteractiveObject();
        nextHint.GetComponent<Text>().text = hint[2];
        yield return new WaitForSeconds(1.8f);
        tutorialTrigger.enabled = true;
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(2);
        nextHint.GetComponent<Text>().text = hint[3];
        tutorialMoveTrigger.enabled = true;
        tutorialMoveTrigger.transform.position = WearController.Instance.transformCache.position;
        yield return new WaitUntil(() => next);


        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(3);
        nextHint.GetComponent<Text>().text = hint[4];
        yield return new WaitUntil(() => next);

        yield return typeWriter.StartShow(text[i]);
        nextHint.GetComponent<Text>().text = hint[0];
        yield return new WaitUntil(() => next);

        //右側選單
        TutorialUI.DOLocalMoveY(1.8f, 0.8f);
        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(4);
        nextHint.GetComponent<Text>().text = hint[5];
        yield return new WaitUntil(() => next);

        //教學結束
        yield return typeWriter.StartShow(text[i]);
        yield return ShowHintImage(3);
        nextHint.GetComponent<Text>().text = hint[0];
        yield return new WaitUntil(() => next);

        TutorialUI.GetComponentInChildren<CanvasGroup>().DOFade(0, 0.5f).OnComplete(
            () =>
            {
                Destroy(gameObject);
                Destroy(gameObject);
            }
        );

        Debug.Log("Tutorial Finish");
    }

    void GenerateInteractiveObject()
    {
        var go = GameController.Instance.SommonBall();
        go.name = "Tutorial";
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
        seq.Join(hintImage.DOFade(1, 0.3f));
        seq.Join(hintImage.rectTransform.DOAnchorPosY(-23.2f, 0.3f));
        return seq.WaitForCompletion();
    }

    bool isTyping = false;
    public void OnTypeStart()
    {
        isTyping = true;
        next = false;
        hintImage.color = new Color(1, 1, 1, 0);
    }
    public void OnTypeFinish()
    {
        isTyping = false;
        i++;

        Debug.Log(i);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Tutorial")
        {
            return;
        }
        if (i != 5)
        {
            return;
        }
        UI_Toast.Instance.ShowToast("很棒！");
        next = true;
        tutorialTrigger.enabled = false;
    }

    public void OnWrapperTriggerEnter(GameObject whoGotHit, Collider other)
    {
        if (other.gameObject.name != "Tutorial")
        {
            return;
        }
        if (i != 6)
        {
            return;
        }
        UI_Toast.Instance.ShowToast("很棒！");
        next = true;
        tutorialMoveTrigger.enabled = false;
    }

    public void OnWrapperTriggerStay(GameObject whoGotHit, Collider other)
    {
    }

    public void OnWrapperTriggerExit(GameObject whoGotHit, Collider other)
    {
    }
}
