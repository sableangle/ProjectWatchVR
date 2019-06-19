using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UI_LevelListPanel : UI_WorldItem
{
    [SerializeField]
    Button prespectiveLevelButton;
    [SerializeField]
    Button frameButton;
    public static UI_LevelListPanel Instance;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        generateHeight = 2.4f;

    }
    bool CheckAnyLevelIsActive
    {
        get
        {
            return AI_PrespectiveLevel.Instance.gameObject.activeSelf || AI_FrameLevel.Instance.gameObject.activeSelf;
        }
    }

    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
        prespectiveLevelButton.OnClickAsObservable().Subscribe(
           _ =>
           {
               if (CheckAnyLevelIsActive)
               {
                   return;
               }
               AI_PrespectiveLevel.Instance.StartPrespectiveLevel();
               Hide();
           }
        );
        frameButton.OnClickAsObservable().Subscribe(
            _ =>
            {
                if (CheckAnyLevelIsActive)
                {
                    return;
                }
                AI_FrameLevel.Instance.StartFrameLevel();
                Hide();
            }
        );
    }
}
