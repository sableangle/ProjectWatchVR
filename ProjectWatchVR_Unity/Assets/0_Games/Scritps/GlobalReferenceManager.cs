using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferenceManager
{
    static AudioClip _PickStartSound;
    public static AudioClip PickStartSound
    {
        get
        {
            if (_PickStartSound == null)
            {
                _PickStartSound = Resources.Load<AudioClip>("pickStart");
            }
            return _PickStartSound;
        }
    }
    static AudioClip _PickEndSound;
    public static AudioClip PickEndSound
    {
        get
        {
            if (_PickEndSound == null)
            {
                _PickEndSound = Resources.Load<AudioClip>("pickEnd");
            }
            return _PickEndSound;
        }
    }
    static AudioClip _clickSound;
    public static AudioClip ClickSound
    {
        get
        {
            if (_clickSound == null)
            {
                _clickSound = Resources.Load<AudioClip>("button-30");
            }
            return _clickSound;
        }
    }

}
