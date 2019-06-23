using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class UI_SettingListPanel : UI_WorldItem
{

    [SerializeField]
    Slider XRotation;


    [SerializeField]
    Slider AllRotation;
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


        XRotation.value = WearController.Instance.lerpSpeedForRotation;
        AllRotation.value = WearController.Instance.lerpParam;
    }

    public void OnXRotationChange()
    {
        WearController.Instance.lerpSpeedForRotation = XRotation.value;
    }
    public void OnAllRotationChange()
    {
        WearController.Instance.lerpParam = AllRotation.value;
    }
}
