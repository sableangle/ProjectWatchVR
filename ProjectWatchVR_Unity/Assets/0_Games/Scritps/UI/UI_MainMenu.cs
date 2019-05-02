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

    public static UI_MainMenu Instance;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

}
