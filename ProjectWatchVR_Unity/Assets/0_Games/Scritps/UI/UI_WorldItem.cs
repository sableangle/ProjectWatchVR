using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UI_WorldItem : MonoBehaviour
{
    // Start is called before the first frame update
    protected Vector3 GetCameraFrontPosition(float height, float front)
    {
        Vector3 p = Camera.main.transform.TransformPoint(new Vector3(0, 0, front));
        p.y = height;
        return p;
    }
    CanvasGroup canvasGroup;

    Transform transfromCache;
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
        transfromCache.position = GetCameraFrontPosition(2.8f, 3);
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
    public void Hide()
    {
        DOTween.Kill(canvasGroup);
        isItemOpen = false;
        canvasGroup.DOFade(0, 0.4f).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
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
