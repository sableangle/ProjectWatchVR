using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;
public class UI_MainMenu : UI_WorldItem
{
    [Header("Buttons")]

    [SerializeField]
    Button summonItemMenu;
    [SerializeField]
    Button groundObjectItemMenu;
    [SerializeField]
    Button levelItemMenu;
    [SerializeField]
    Button settingItemMenu;

    public static UI_MainMenu Instance;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        generateHeight = 2.8f;
    }
    protected override void Start()
    {
        base.Start();
        summonItemMenu.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide(UI_SummonListPanel.Instance.Show);
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );

        groundObjectItemMenu.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide(UI_GroundObjectListPanel.Instance.Show);
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );

        levelItemMenu.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide(UI_LevelListPanel.Instance.Show);
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );

        settingItemMenu.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide(UI_SettingListPanel.Instance.Show);
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );
    }

    public delegate void _OnMenuOpen();
    public _OnMenuOpen OnMenuOpen;
    public new void Swtich()
    {
        base.Swtich();
        if (OnMenuOpen != null)
        {
            OnMenuOpen();
        }
    }
}
