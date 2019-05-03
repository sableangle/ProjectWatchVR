﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroundObjectListPanel : UI_WorldItem
{
    [SerializeField]
    Button closeButton;

    [SerializeField]
    Button flowerButton;
    [SerializeField]
    Button grassButton;

    public static UI_GroundObjectListPanel Instance;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        generateHeight = 2.4f;

    }

    void Start()
    {
        closeButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide();
            }
        );
        flowerButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                GameController.Instance.CreateFlower();
            }
        );
        grassButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                GameController.Instance.CreateGrass();
            }
        );
    }
}
