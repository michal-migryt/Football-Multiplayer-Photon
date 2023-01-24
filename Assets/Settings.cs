using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public int volume;
    public int brightness;
    public Settings(int volume, int brightness){
        this.volume = volume;
        this.brightness = brightness;
    }
}
