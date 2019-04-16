using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ConnectModels : MonoBehaviour
{
    public static Quaternion rotation;
    public static Vector3 accelerometer;
    public static Vector3 rotationEular;

    public static Vector2 screenPosition;
    public class WatchSensor : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //Debug.Log("OnMessage"+e.Data);
            var msg = e.Data.Split('@');
            // 四元數
            float qua_w, qua_x, qua_y, qua_z;

            var qua = msg[0].Split('_');
            float.TryParse(qua[0], out qua_x);
            float.TryParse(qua[0], out qua_y);
            float.TryParse(qua[2], out qua_z);
            float.TryParse(qua[3], out qua_w);

            rotation = new Quaternion(qua_x, qua_y, qua_z, qua_w);


            //加速度季

            var acc = msg[1].Split('_');
            float acc_x, acc_y, acc_z;
            float.TryParse(acc[0], out acc_x);
            float.TryParse(acc[0], out acc_y);
            float.TryParse(acc[2], out acc_z);

            accelerometer = new Vector3(acc_x, acc_y, acc_z);

            //rotationEular = new Vector3(x,y,z);
        }
    }

    public enum InputType
    {
        Start, End, Move, Up, Down, Left, Right, None
    }
    private InputType lastWatchMove = InputType.None;
    public delegate void _OnWatchMoveChange(Vector2 pos);
    public static event _OnWatchMoveChange OnWatchMoveChange;
    public class WatchInput : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //Debug.Log("OnMessage"+e.Data);
            var msg = e.Data.Split(',');
            float x, y;
            float.TryParse(msg[1], out x);
            float.TryParse(msg[2], out y);
            Debug.LogFormat("{0} , x : {1} , y : {2}", msg[0], x, y);
            screenPosition = new Vector2(x, y);
            // if (msg[0].Contains("End"))
            // {
            // 	case 
            // }
            // if (OnWatchMoveChange != null)
            // {
            //     OnWatchMoveChange(screenPosition);
            // }
        }
    }
}