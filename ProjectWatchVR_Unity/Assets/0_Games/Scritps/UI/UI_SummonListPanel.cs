using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class UI_SummonListPanel : UI_WorldItem
{
   
    public static UI_SummonListPanel Instance;

    [SerializeField]
    Button summonBoxButton;
    [SerializeField]
    Button summonBallButton;
    [SerializeField]
    Button summonFoodButton;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        generateHeight = 2.4f;

    }
    protected override void  Start()
    {
       base.Start();
        summonBoxButton.OnClickAsObservable().Subscribe(
           _ =>
           {
               GameController.Instance.SommonBox();
           }
       );
        summonBallButton.OnClickAsObservable().Subscribe(
           _ =>
           {
               GameController.Instance.SommonBall();
           }
       );
        summonFoodButton.OnClickAsObservable().Subscribe(
           _ =>
           {
               GameController.Instance.SommonFood();
           }
       );
    }

}
