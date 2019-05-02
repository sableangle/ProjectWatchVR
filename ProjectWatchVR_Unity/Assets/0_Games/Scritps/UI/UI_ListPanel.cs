using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class UI_ListPanel : UI_WorldItem
{
    [SerializeField]
    Button closeButton;

    void Start()
    {
        closeButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                Hide();
            }
        );
    }

}
