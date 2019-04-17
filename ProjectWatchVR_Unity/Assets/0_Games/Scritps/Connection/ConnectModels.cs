using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class InputModels : MonoBehaviour
{



    public static Quaternion rotation;
    public static Vector3 accelerometer;

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
            float.TryParse(qua[1], out qua_y);
            float.TryParse(qua[2], out qua_z);
            float.TryParse(qua[3], out qua_w);

            rotation = new Quaternion(qua_x, qua_y, qua_z, qua_w);


            //加速度季

            var acc = msg[1].Split('_');
            float acc_x, acc_y, acc_z;
            float.TryParse(acc[0], out acc_x);
            float.TryParse(acc[1], out acc_y);
            float.TryParse(acc[2], out acc_z);

            accelerometer = new Vector3(acc_x, acc_y, acc_z);
        }
    }

    public delegate void _OnWatchMoveChange(Vector2 pos);
    public static event _OnWatchMoveChange OnWatchMoveChange;

    public enum Buttons
    {
        None = -1, Up = 0, Down = 1, Left = 2, Right = 3, Center = 4
    }
    public enum ButtonAction
    {
        None = -1, Up = 0, Down = 1
    }
    static Buttons lastButtons = Buttons.None;
    static ButtonAction lastButtonAction = ButtonAction.None;
    public class WatchInput : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data.Split(',');
            //Debug.Log(e.Data);

            float x, y;

            float.TryParse(msg[1], out x);
            float.TryParse(msg[2], out y);

            screenPosition = new Vector2(x, y);

            if (msg[0].Contains("End") || msg[0].Contains("Up"))
            {
                screenPosition = new Vector2(0, 0);
            }

            if (msg[0].Contains("Down") || msg[0].Contains("Up"))
            {
                var action = msg[0].Split('_');
                try
                {
                    lastButtons = (Buttons)System.Enum.Parse(typeof(Buttons), action[1]);
                }
                catch
                {
                    lastButtons = Buttons.None;
                }
                try
                {
                    lastButtonAction = (ButtonAction)System.Enum.Parse(typeof(ButtonAction), action[0]);
                }
                catch
                {
                    lastButtonAction = ButtonAction.None;
                }
            }
            else
            {

            }
        }
    }

    public static bool GetWatchButtonDown(int index)
    {
        return GetWatchButtonDown((Buttons)index);

    }
    public static bool GetWatchButtonDown(Buttons index)
    {
        var resutl = lastButtons == index && lastButtonAction == ButtonAction.Down;
        if (resutl)
        {
            lastButtons = Buttons.None;
            lastButtonAction = ButtonAction.None;
        }
        return resutl;
    }

    public static bool GetWatchButtonUp(int index)
    {
        return GetWatchButtonUp((Buttons)index);
    }
    public static bool GetWatchButtonUp(Buttons index)
    {
        var resutl = lastButtons == index && lastButtonAction == ButtonAction.Up;
        if (resutl)
        {
            lastButtons = Buttons.None;
            lastButtonAction = ButtonAction.None;
        }
        return resutl;
    }
}