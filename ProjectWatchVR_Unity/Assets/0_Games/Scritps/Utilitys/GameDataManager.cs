using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    const string PP_X_Rotation_Use_GRYO_Setting = "PP_X_Rotation_Setting";
    // true ACC false Gryo
    public bool XRotationSetting
    {
        get
        {
            return GameDataBridge.GetBool(PP_X_Rotation_Use_GRYO_Setting, false);
        }
        set
        {
            GameDataBridge.SetBool(PP_X_Rotation_Use_GRYO_Setting, value);
        }
    }
}
