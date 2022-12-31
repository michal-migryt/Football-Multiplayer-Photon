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
    public KeyCode StopShootingKeyCode{get;set;}

    // Start is called before the first frame update
    // void Start()
    // {
    // //    StreamReader streamReader = new StreamReader("Player Inputs");
    // //    string jsonString = System.IO.File.ReadAllText("Player Inputs");
    // //    JsonUtility.FromJson(jsonString);
    // //    KeyCode.
    // }
    public KeycodeManager()
    {
        customKeycodes = new CustomKeycodes();
        ShootKeyCode = KeyCode.Space;
        CurveKeyCode = KeyCode.F;
        ChipKeyCode = KeyCode.A;
        StopShootingKeyCode = KeyCode.S;
        customKeycodes.UpdateKeyCodes(this);
    }
    public KeycodeManager(CustomKeycodes customKeycodes)
    {
        this.customKeycodes = customKeycodes;
        ShootKeyCode = (KeyCode) customKeycodes.shootKeyCode;
        CurveKeyCode = (KeyCode) customKeycodes.curveKeyCode;
        ChipKeyCode = (KeyCode) customKeycodes.chipKeyCode;
        StopShootingKeyCode = (KeyCode) customKeycodes.stopShootingKeyCode;
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
