using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    void Awake()
    {
        Instance = this;
    }


    #region SommonObject
    public enum SommonObject
    {
        Box = 0, Ball = 1, Food = 2
    }

    public GameObject[] SommonPrefabs;
    public void SommonBox()
    {
        SommonGameObject((int)SommonObject.Box);
    }
    public void SommonBall()
    {
        SommonGameObject((int)SommonObject.Ball);
    }
    public void SommonFood()
    {
        SommonGameObject((int)SommonObject.Food);
    }

    public void SommonGameObject(int i)
    {
        var pos = Utilitys.GetCameraFrontPosition(5, 3);
        var go = Instantiate<GameObject>(SommonPrefabs[i],
        pos,
        Quaternion.identity);
    }
    #endregion

    #region GroundObject

    public GameObject[] GroundObjectPrefabs;
    public enum GroundObject
    {
        Flower = 0, Grass = 1
    }

    public void CreateFlower()
    {
        CreateGroundObject((int)GroundObject.Flower);
    }
    public void CreateGrass()
    {
        CreateGroundObject((int)GroundObject.Grass);
    }

    public void CreateGroundObject(int i)
    {
        if (WearController.isPicking)
        {
            return;
        }
        var go = Instantiate<GameObject>(GroundObjectPrefabs[i],
        WearController.Instance.pointer.position,
        Quaternion.identity);
        var pickable = go.GetComponent<IPickable>();
        WearController.Instance.SetCurrentPickable(pickable);
        WearController.Instance.PickStart();
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UI_MainMenu.Instance.Swtich();
        }
    }


}
