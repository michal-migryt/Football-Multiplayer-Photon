using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomKeycodes
{
    public int shootKeyCode;
    public int curveKeyCode;
    public int chipKeyCode;
    public int stopShootingKeyCode;
    public int flatKeyCode;

    public void UpdateKeyCodes(KeycodeManager keycodeManager)
    {
        shootKeyCode = (int) keycodeManager.ShootKeyCode;
        curveKeyCode = (int) keycodeManager.CurveKeyCode;
        chipKeyCode = (int) keycodeManager.ChipKeyCode;
        stopShootingKeyCode = (int) keycodeManager.StopShootingKeyCode;
        flatKeyCode = (int) keycodeManager.FlatKeyCode;
    }
}
