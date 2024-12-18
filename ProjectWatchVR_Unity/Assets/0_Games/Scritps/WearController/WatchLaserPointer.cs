﻿using System;
using UnityEngine;
using Wacki;


public class WatchLaserPointer : IUILaserPointer
{
    public static WatchLaserPointer Instance;
    WearController wearController;
    public void Awake()
    {
        wearController = GetComponent<WearController>();
        VRInputReciver.OnWatchButtonDown += OnWatchButtonDown;
        VRInputReciver.OnWatchButtonUp += OnWatchButtonUp;

        Instance = this;
    }

    private void OnWatchButtonUp(VRInputReciver.Buttons btn)
    {
        buttonState = false;
    }

    private void OnWatchButtonDown(VRInputReciver.Buttons btn)
    {
        if (btn == VRInputReciver.Buttons.Center) buttonState = true;
    }

    public bool buttonState = false;
    bool _prevButtonState = false;
    bool _buttonChanged = false;

    protected override void Update()
    {
        base.Update();
        if (buttonState == _prevButtonState)
        {
            _buttonChanged = false;
        }
        else
        {
            _buttonChanged = true;
            _prevButtonState = buttonState;
        }
    }
    public override bool ButtonDown()
    {
        if (wearController.editorSimlator)
        {
            return Input.GetMouseButtonDown(0);
        }
        else
        {
            //return VRInputReciver.GetWatchButtonDown(VRInputReciver.Buttons.Center);
            return _buttonChanged && buttonState;
        }
    }

    public override bool ButtonUp()
    {
        if (wearController.editorSimlator)
        {
            return Input.GetMouseButtonUp(0);
        }
        else
        {
            //return VRInputReciver.GetWatchButtonUp(VRInputReciver.Buttons.Center);
            return _buttonChanged && !buttonState;
        }
    }

    public override void OnEnterControl(GameObject control)
    {
      
    }

    public override void OnExitControl(GameObject control)
    {
        
    }
}

