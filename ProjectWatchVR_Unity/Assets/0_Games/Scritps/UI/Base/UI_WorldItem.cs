using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UI_WorldItem : MonoBehaviour, IPickable
{
    // Start is called before the first frame update
    [SerializeField]
    Button closeButton;
    [SerializeField]
    Button moveButton;

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

    protected virtual void Start()
    {
        closeButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide();
            }
        );

        moveButton.OnPointerDownAsObservable().Subscribe(
             _ =>
            {
                if (WearController.isPicking)
                {
                    return;
                }
                WearController.Instance.SetCurrentPickable(this);
                WearController.Instance.PickStart();
            }
        );

        moveButton.OnPointerUpAsObservable().Subscribe(
            _ =>
            {
                WearController.Instance.PickEnd();
            }
        );
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

    void Update()
    {
        if (isPick)
        {
            transfromCache.position = picker.position;
        }
    }
    #region  Pickable
    bool isPick = false;
    private Transform picker;

    public void OnPickStart(Transform pickPointer)
    {
        picker = pickPointer;
        isPick = true;
    }

    public void OnPickFinish()
    {
        //transfromCache.SetParent(null);

        isPick = false;
    }

    public void OnPointEnter()
    {
    }

    public void OnPointOut()
    {
    }

    public Transform GetTransform()
    {
        return transfromCache;
    }
    public MonoBehaviour GetBehaviour()
    {
        return this;
    }
    #endregion
}
