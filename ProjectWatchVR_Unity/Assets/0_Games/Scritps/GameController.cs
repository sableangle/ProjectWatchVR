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

    #endregion


}
