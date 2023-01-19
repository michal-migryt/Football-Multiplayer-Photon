using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycodeManager
{
    private CustomKeycodes customKeycodes;
    public KeyCode ShootKeyCode{get;set;}
    public KeyCode CurveKeyCode{get;set;}
    public KeyCode ChipKeyCode{get;set;}
    public KeyCode FlatKeyCode{get;set;}
    public KeyCode StopShootingKeyCode{get;set;}

    public KeycodeManager()
    {
        customKeycodes = new CustomKeycodes();
        ShootKeyCode = KeyCode.Space;
        CurveKeyCode = KeyCode.F;
        ChipKeyCode = KeyCode.A;
        StopShootingKeyCode = KeyCode.S;
        FlatKeyCode = KeyCode.D;
        customKeycodes.UpdateKeyCodes(this);
    }
    public KeycodeManager(CustomKeycodes customKeycodes)
    {
        this.customKeycodes = customKeycodes;
        ShootKeyCode = (KeyCode) customKeycodes.shootKeyCode;
        CurveKeyCode = (KeyCode) customKeycodes.curveKeyCode;
        ChipKeyCode = (KeyCode) customKeycodes.chipKeyCode;
        StopShootingKeyCode = (KeyCode) customKeycodes.stopShootingKeyCode;
        FlatKeyCode = (KeyCode) customKeycodes.flatKeyCode;
    }
    public static KeycodeManager CreateFromJSON(string jsonString){
        if(jsonString != "")
        return new KeycodeManager(JsonUtility.FromJson<CustomKeycodes>(jsonString));
        else
        return new KeycodeManager();
    }
    public string ToJSON(){
        return JsonUtility.ToJson(customKeycodes);
    }
}
