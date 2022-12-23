using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShotSlider : MonoBehaviour
{
    [SerializeField] private Image shotPowerSliderFill;
    private Slider shotPowerSlider;
    private Color fillColor;
    public void InitializeShotSlider(){
        shotPowerSlider = GetComponent<Slider>();
        DisableShotSlider();
    }
    public void EnableShotSlider(Vector3 sliderNewPosition){
        shotPowerSlider.transform.position = sliderNewPosition;
        gameObject.SetActive(true);
        shotPowerSlider.enabled = true;
    }
    public void UpdateShotSlider(Vector3 sliderNewPosition, float shotPower){
        shotPowerSlider.transform.position = sliderNewPosition;
        shotPowerSlider.value = shotPower;
        float red, green, blue;
        red = 5.1f * shotPower;
        green = 255 - 5.1f*(shotPower-50);
        blue = 0;
        red = Mathf.Clamp(red, 0, 255);
        green = Mathf.Clamp(green, 0, 255);
        fillColor = new Color32((byte)red, (byte)green, (byte) blue, 255);
        shotPowerSliderFill.color = fillColor;
    }
    public void DisableShotSlider(){
        gameObject.SetActive(false);
        shotPowerSlider.enabled = false;
        shotPowerSlider.value = 1;
    }
}
