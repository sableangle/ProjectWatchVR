using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroundObjectListPanel : UI_WorldItem
{


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

    protected override void Start()
    {
        base.Start();
        flowerButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                GameController.Instance.CreateFlower();
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );
        grassButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                GameController.Instance.CreateGrass();
                AudioManager.PlaySFX(GlobalReferenceManager.ClickSound);
            }
        );
    }
}
