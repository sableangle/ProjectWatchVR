using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class UI_ScreenSpace : MonoBehaviour
{
    public static UI_ScreenSpace Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Hint_Reset.gameObject.SetActive(false);
    }
    [Header("Reset Panel")]
    [SerializeField]
    CanvasGroup Hint_Reset;
    [SerializeField]
    Image Hint_ResetProgress;
    Coroutine ResetHintCoroutine;
    public void ShowHint_Reset()
    {
        ResetHintCoroutine = StartCoroutine(StartResetUIConter());
        Hint_Reset.DOFade(1, 0.4f).OnStart(() =>
        {
            Hint_Reset.alpha = 0;
            Hint_Reset.gameObject.SetActive(true);
        });
    }
    public void ExitHint_Reset()
    {
        StopCoroutine(ResetHintCoroutine);
        Hint_Reset.DOFade(0, 0.4f).OnComplete(() =>
        {
            Hint_Reset.gameObject.SetActive(false);
        });
    }
    public IEnumerator StartResetUIConter()
    {
        Hint_ResetProgress.fillAmount = 0;
        while (WearController.Instance.GetResetSensor() && Hint_ResetProgress.fillAmount < 1)
        {
            Hint_ResetProgress.fillAmount = WearController.Instance.GetResetSensorTimer() / WearController.resetNeedTime;
            yield return null;
        }
        ExitHint_Reset();
    }
}
