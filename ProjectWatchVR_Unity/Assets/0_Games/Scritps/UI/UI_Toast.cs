using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Toast : MonoBehaviour
{
    public static UI_Toast Instance;
    void Awake()
    {
        Instance = this;
        text = GetComponentInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0;
    }
    Text text;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    Coroutine c;
    public void ShowToast(string msg, bool queueIfIsShowing = true)
    {
        if (c != null && queueIfIsShowing == false)
        {
            return;
        }
        msgQueue.Enqueue(msg);
        if (c != null)
        {
            return;
        }
        c = StartCoroutine(ShowToastRunner());
    }

    Queue<string> msgQueue = new Queue<string>();

    IEnumerator ShowToastRunner()
    {
        if (msgQueue.Count <= 0)
        {
            c = null;
            yield break;
        }
        var msg = msgQueue.Dequeue();
        text.text = msg;
        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = new Vector2(0, -220);
        rectTransform.DOAnchorPosY(-180, 0.4f);
        yield return canvasGroup.DOFade(1, 0.4f).WaitForCompletion();
        yield return new WaitForSeconds(3);
        yield return canvasGroup.DOFade(0, 0.4f).WaitForCompletion();
        yield return ShowToastRunner();
    }
}
