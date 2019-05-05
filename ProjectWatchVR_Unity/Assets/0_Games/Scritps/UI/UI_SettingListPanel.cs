using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class UI_SettingListPanel : UI_WorldItem
{


    public static UI_SettingListPanel Instance;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        generateHeight = 2.4f;

    }

    protected override void Start()
    {
        base.Start();
    }

}
