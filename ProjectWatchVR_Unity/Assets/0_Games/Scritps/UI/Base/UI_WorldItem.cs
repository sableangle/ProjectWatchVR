using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UI_WorldItem : MonoBehaviour
{
    // Start is called before the first frame update
    

    CanvasGroup canvasGroup;
    Transform transfromCache;
    protected float generateHeight = 2.8f;
    bool isItemOpen = false;
    protected virtual void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        isItemOpen = false;
        transfromCache = transform;
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        transfromCache.position = Utilitys.GetCameraFrontPosition(generateHeight, 3);
        DOTween.Kill(canvasGroup);
        isItemOpen = true;
        canvasGroup.DOFade(1, 0.4f).OnStart(
            () =>
            {
                gameObject.SetActive(true);
                canvasGroup.alpha = 0;
            }
        );
    }

    Tween hideTween;
    public void Hide(System.Action OnComplete = null)
    {
        DOTween.Kill(canvasGroup);
        isItemOpen = false;
        canvasGroup.DOFade(0, 0.4f).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                }
            }
        );
    }

    public void Swtich()
    {
        if (isItemOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}
