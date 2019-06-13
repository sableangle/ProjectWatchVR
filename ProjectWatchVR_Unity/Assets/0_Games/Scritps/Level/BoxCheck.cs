using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class BoxCheck : MonoBehaviour
{
    public enum Box_Pos_Type
    {
        Big, Small
    }
    public Box_Pos_Type boxType = Box_Pos_Type.Big;
    Renderer renderer
    {
        get
        {
            return GetComponent<Renderer>();
        }
    }
    public bool isPass = false;
    const string colorKey = "_Color";
    // Start is called before the first frame update
    void Start()
    {
        isPass = false;
        renderer.material.SetColor(colorKey, new Color(1, 0, 0, 1));
        this.ObserveEveryValueChanged(m => m.isPass).Subscribe(
            isP =>
            {
                if (isP)
                {
                    //Do something
                    renderer.material.SetColor(colorKey, new Color(0, 1, 0, 1));
                }
                else
                {
                    //Do something
                    renderer.material.SetColor(colorKey, new Color(1, 0, 0, 1));
                }

            }
        );
    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
        isPass = false;
    }

    void OnTriggerStay(Collider other)
    {

        Transform t = other.transform;

        var dis = (new Vector2(t.position.x, t.position.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude;
        Debug.Log(dis);



        if (boxType == Box_Pos_Type.Big)
        {
            if (dis > 0.0003f)
            {
                isPass = false;
                return;
            }
        }
        else if (boxType == Box_Pos_Type.Small)
        {
            if (dis > 0.0002f)
            {
                isPass = false;
                return;
            }
        }

        if (isPass)
        {
            return;
        }

        if (boxType == Box_Pos_Type.Big)
        {
            if (t.localScale.x < 0.465f || t.localScale.x > 0.52f)
            {
                return;
            }
        }
        else if (boxType == Box_Pos_Type.Small)
        {
            if (t.localScale.x < 0.25f || t.localScale.x > 0.29f)
            {
                return;
            }
        }



        isPass = true;
    }
}
