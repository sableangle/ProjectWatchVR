using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class UI_ScreenSpace : MonoBehaviour
{
    public static UI_ScreenSpace Instance;
    public Camera camera;
    private Canvas canvas;
    void Awake()
    {
        Instance = this;
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = camera;
    }

    void Start()
    {
        Hint_Reset.gameObject.SetActive(false);
        Hint_Setting.gameObject.SetActive(false);
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = camera;
    }
    void Update()
    {
        if (canvas.worldCamera != camera)
        {
            canvas.worldCamera = camera;
        }
    }
    [Header("Reset Panel")]
    [SerializeField]
    CanvasGroup Hint_Reset;
    [SerializeField]
    Image Hint_ResetProgress;
    Coroutine ResetHintCoroutine;
    public void ShowHint_Reset()
    {
        DOTween.Kill(Hint_Reset);
        ResetHintCoroutine = StartCoroutine(StartResetUIConter());
        Hint_Reset.DOFade(1, 0.4f).OnStart(() =>
        {
            Hint_Reset.alpha = 0;
            Hint_Reset.gameObject.SetActive(true);
        });
    }
    public void ExitHint_Reset()
    {
        DOTween.Kill(Hint_Reset);
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
            Hint_ResetProgress.fillAmount = WearController.Instance.GetResetSensorTimer() / GlobalDefine.resetNeedTime;
            yield return null;
        }
        ExitHint_Reset();
    }

    [Header("Setting Panel")]
    [SerializeField]
    CanvasGroup Hint_Setting;
    [SerializeField]
    Image Hint_SettingProgress;
    Coroutine SettingHintCoroutine;
    public void ShowHint_Setting()
    {
        DOTween.Kill(Hint_Setting);
        SettingHintCoroutine = StartCoroutine(StartSettingUIConter());
        Hint_Setting.DOFade(1, 0.4f).OnStart(() =>
        {
            Hint_Setting.alpha = 0;
            Hint_Setting.gameObject.SetActive(true);
        });
    }
    public void ExitHint_Setting()
    {
        DOTween.Kill(Hint_Setting);
        StopCoroutine(SettingHintCoroutine);
        Hint_Setting.DOFade(0, 0.4f).OnComplete(() =>
        {
            Hint_Setting.gameObject.SetActive(false);
        });
    }
    public IEnumerator StartSettingUIConter()
    {
        Hint_SettingProgress.fillAmount = 0;
        while (WearController.Instance.GetSettingSensor() && Hint_SettingProgress.fillAmount < 1)
        {
            Hint_SettingProgress.fillAmount = WearController.Instance.GetSettingTimer() / GlobalDefine.settingNeedTime;
            yield return null;
        }
        ExitHint_Setting();
    }
}
