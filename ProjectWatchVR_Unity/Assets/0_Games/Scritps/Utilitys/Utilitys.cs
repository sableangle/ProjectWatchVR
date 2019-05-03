using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilitys 
{
    public static Vector3 GetCameraFrontPosition(float height, float front)
    {
        Vector3 p = Camera.main.transform.TransformPoint(new Vector3(0, 0, front));
        p.y = height;
        return p;
    }
}
