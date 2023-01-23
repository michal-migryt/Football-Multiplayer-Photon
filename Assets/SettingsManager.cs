using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO: APPLY SETTINGS OR REVERT THEM
public class SettingsManager : MonoBehaviour
{
    enum ButtonType{NONE, SHOOT, FLAT, CURVE, CHIP, STOP};
    public static SettingsManager instance;
    private KeycodeManager keycodeManager;
    private CustomKeycodes customKeycodes, customKeycodesCopy;
    [SerializeField] private Slider brightnessSlider, volumeSlider;
    [SerializeField] private Button shootButton, flatButton, curveButton, chippedButton, stopButton;
    ButtonType chosenButtonType = ButtonType.NONE;
    private Button chosenButton;
    [SerializeField]private int animationTime;
    public int brightness{get;set;}
    public int volume{get;set;}
    // TODO: stop character from moving when settings are open, maybe do it also when menu is open
    public bool isChangingSettings{get;set;}
    //TODO: possibly check if user made change and make a popup appear if he will try to exit settings without applying
    private bool isMappingKey = false, applied = false, madeChanges = false;
    private int brightnessCopy, volumeCopy;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.25f);

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
        {
            instance = this;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        keycodeManager = KeycodeManager.CreateFromJSON(FileManager.instance.ReadFromPlayerInputFile());
        
        InitializeButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMappingKey)
            return;
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyCode))
            {
                Debug.Log("happened");
                MapButton(keyCode);
            }
        }
    }
    private void InitializeButtons()
    {
        shootButton.onClick.AddListener(() => {SelectButton(shootButton, ButtonType.SHOOT);});
        shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ShootKeyCode.ToString();
        flatButton.onClick.AddListener(() => {SelectButton(flatButton, ButtonType.FLAT);});
        flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.FlatKeyCode.ToString();
        curveButton.onClick.AddListener(() => {SelectButton(curveButton, ButtonType.CURVE);});
        curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.CurveKeyCode.ToString();
        chippedButton.onClick.AddListener(() => {SelectButton(chippedButton, ButtonType.CHIP);});
        chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ChipKeyCode.ToString();
        stopButton.onClick.AddListener(() => {SelectButton(stopButton, ButtonType.STOP);});
        stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.StopShootingKeyCode.ToString();
    }
    private void UpdateButtons()
    {
        shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ShootKeyCode.ToString();
        flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.FlatKeyCode.ToString();
        curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.CurveKeyCode.ToString();
        chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ChipKeyCode.ToString();
        stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.StopShootingKeyCode.ToString();
    }
    private void MapButton(KeyCode keyCode)
    {
        switch(chosenButtonType){

            case ButtonType.SHOOT:
            keycodeManager.ShootKeyCode = keyCode;
            shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.FLAT:
            keycodeManager.FlatKeyCode = keyCode;
            flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.CURVE:
            keycodeManager.CurveKeyCode = keyCode;
            curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.CHIP:
            keycodeManager.ChipKeyCode = keyCode;
            chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.STOP:
            keycodeManager.StopShootingKeyCode = keyCode;
            stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;
        }
        isMappingKey = false;
    }
    public void OnRevertSettings()
    {
        keycodeManager.UpdateKeyCodes(customKeycodesCopy);
        UpdateButtons();
        brightness = brightnessCopy;
        volume = volumeCopy;
    }
    public KeycodeManager GetKeycodeManager()
    {
        return keycodeManager;
    }
    public void OnApplyButton()
    {
        applied = true;
        FileManager.instance.SaveToPlayerInputFile(keycodeManager);
    }

    public void OnExitSettings()
    {
        if(applied)
            applied = false;
        else{
            OnRevertSettings();
        }
        isChangingSettings = false;

    }
    public void OnOpenSettings()
    {
        brightnessCopy = brightness;
        volumeCopy = volume;
        customKeycodesCopy = keycodeManager.GetCustomKeycodes();
    }
    public void UpdateBrightness(float value)
    {
        brightness = (int)value * 100;
    }
    public void UpdateVolume(float value)
    {
        volume = (int)value * 100;
    }
    private void ResetButton()
    {

        ColorBlock colorBlock = chosenButton.colors;
        colorBlock.normalColor = Color.white;
        colorBlock.selectedColor = Color.white;
        chosenButton = null;
        chosenButtonType = ButtonType.NONE;
    }
    private void SelectButton(Button button, ButtonType buttonType)
    {
        chosenButton = button;
        chosenButtonType = buttonType;
        isMappingKey = true;
        StartCoroutine(PlayAnimation());
        
    }
    private IEnumerator PlayAnimation()
    {
        Debug.Log(chosenButton.gameObject.name);
        ColorBlock colorBlock = chosenButton.colors;
        // colorBlock.normalColor = Color.black;
        chosenButton.colors = colorBlock;
        int direction = -1;
        yield return waitForSeconds;
        while (isMappingKey)
        {
            Debug.Log(chosenButton.gameObject.name);
            colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g + direction, colorBlock.normalColor.b + direction);
            colorBlock.selectedColor = new Color(colorBlock.selectedColor.r, colorBlock.selectedColor.g + direction, colorBlock.selectedColor.b + direction);
            Debug.Log(colorBlock.selectedColor);
            chosenButton.colors = colorBlock;
            if (colorBlock.selectedColor.g <= 200f || colorBlock.selectedColor.g >= 254)
                direction = -direction;
            yield return waitForSeconds;
        }
        ResetButton();
    }
}
