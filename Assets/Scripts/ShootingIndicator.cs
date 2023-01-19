using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingIndicator : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    Material material;
    
    public void Init()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        material = meshRenderer.material;
        material.color = Color.green;
        
    }
    public void Enable()
    {
        meshRenderer.enabled = true;
    }
    public void UpdateColor(float shotPower)
    {
        float red, green, blue;
        red = 5.1f * shotPower;
        green = 255 - 5.1f*(shotPower-50);
        blue = 0;
        red = Mathf.Clamp(red, 0, 255);
        green = Mathf.Clamp(green, 0, 255);
        material.color = new Color32((byte)red, (byte)green, (byte) blue, 255);
    }
    public void Disable()
    {
        meshRenderer.enabled = false;
        material.color = Color.green;
    }

    
}
