using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UI_MainMenu : MonoBehaviour
{
    public static UI_MainMenu Instance;

    [SerializeField]
    CanvasGroup canvasGroup;

    bool menuIsOpen = false;
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
        menuIsOpen = false;
    }

    public void Show()
    {
        DOTween.Kill(canvasGroup);
        menuIsOpen = true;
        canvasGroup.DOFade(1, 0.4f).OnStart(
            () =>
            {
                gameObject.SetActive(true);
                canvasGroup.alpha = 0;
            }
        );
    }

    Tween hideTween;
    public void Hide()
    {
        DOTween.Kill(canvasGroup);
        menuIsOpen = false;
        canvasGroup.DOFade(0, 0.4f).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
            }
        );
    }

    public void Swtich()
    {
        if (menuIsOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}
